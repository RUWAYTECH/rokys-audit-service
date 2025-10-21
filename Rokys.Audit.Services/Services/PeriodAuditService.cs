using FluentValidation;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.Common.Helpers;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.PeriodAudit;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.PeriodAudit;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class PeriodAuditService : IPeriodAuditService
    {
        private readonly IRepository<PeriodAudit> _repository;
        private readonly IValidator<PeriodAuditRequestDto> _validator;
        private readonly ILogger<PeriodAuditService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditStatusRepository _auditStatusRepository;
        private readonly IPeriodAuditGroupResultRepository _periodAuditGroupResultRepository;
        private readonly IScaleCompanyRepository _scaleCompanyRepository;
        private readonly IInboxItemsRepository _inboxItemsRepository;
        private readonly IUserReferenceRepository _userReferenceRepository;

        public PeriodAuditService(
            IRepository<PeriodAudit> repository,
            IValidator<PeriodAuditRequestDto> validator,
            ILogger<PeriodAuditService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IAuditStatusRepository auditStatusRepository,
            IPeriodAuditGroupResultRepository periodAuditGroupResultRepository,
            IScaleCompanyRepository scaleCompanyRepository,
            IInboxItemsRepository inboxItemsRepository,
            IUserReferenceRepository userReferenceRepository)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _auditStatusRepository = auditStatusRepository;
            _periodAuditGroupResultRepository = periodAuditGroupResultRepository;
            _scaleCompanyRepository = scaleCompanyRepository;
            _inboxItemsRepository = inboxItemsRepository;
            _userReferenceRepository = userReferenceRepository;
        }

        public async Task<ResponseDto<PeriodAuditResponseDto>> Create(PeriodAuditRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                var auditStatus = await _auditStatusRepository.GetFirstOrDefaultAsync(filter: x => x.Code == AuditStatusCode.Pending && x.IsActive);

                var currentUser = _httpContextAccessor.CurrentUser();
                var currentUserName = currentUser?.UserName ?? "system";
                var entity = _mapper.Map<PeriodAudit>(requestDto);
                // Obtener el último código existente
                var lastCode = _repository.Get()
                    .OrderByDescending(x => x.CreationDate)
                    .Select(x => x.CorrelativeNumber)
                    .FirstOrDefault();
                var currentYear = DateTime.Now.Year;
                var nextCode = CodeGeneratorHelper.GenerateNextCode("CSR-" + currentYear, lastCode, 4);
                entity.CorrelativeNumber = nextCode;
                entity.StatusId = auditStatus.AuditStatusId;
                entity.CreateAudit(currentUserName);
                _repository.Insert(entity);
                await _unitOfWork.CommitAsync();
                var entityCreate = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditId == entity.PeriodAuditId && x.IsActive,
                    includeProperties: [x => x.Store.Enterprise, x => x.Administrator, x => x.Assistant, x => x.OperationManager, x => x.FloatingAdministrator, x => x.ResponsibleAuditor, x => x.AuditStatus, su => su.Supervisor]);
                // Crear registro en InboxItems: prev user = administrador, next user = auditor responsable
                try
                {
                    if (_inboxItemsRepository != null)
                    {
                        Guid? prevUserId = null;
                        Guid? nextUserId = null;
                        if (entity.AdministratorId.HasValue)
                        {
                            var adminRef = await _userReferenceRepository.GetByUserIdAsync(entity.AdministratorId.Value);
                            if (adminRef != null) prevUserId = adminRef.UserReferenceId;
                        }
                        if (entity.ResponsibleAuditorId.HasValue)
                        {
                            var auditorRef = await _userReferenceRepository.GetByUserIdAsync(entity.ResponsibleAuditorId.Value);
                            if (auditorRef != null) nextUserId = auditorRef.UserReferenceId;
                        }

                        var inboxItem = new InboxItems
                        {
                            PeriodAuditId = entity.PeriodAuditId,
                            PrevUserId = prevUserId,
                            NextUserId = nextUserId,
                            PrevStatusId = (await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.Pending && f.IsActive))?.AuditStatusId,
                            NextStatusId = (await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.InProgress && f.IsActive))?.AuditStatusId,
                            Comments = "Auditoría creada",
                            IsActive = true
                        };
                        var currentUser2 = _httpContextAccessor.CurrentUser();
                        var currentUser2Name = currentUser2?.UserName ?? "system";
                        // record who created the inbox entry and the action (only if user exists in UserReference)
                        if (currentUser2 != null)
                        {
                            var actorRef = await _userReferenceRepository.GetByUserIdAsync(currentUser2.UserId);
                            if (actorRef != null)
                                inboxItem.UserId = actorRef.UserReferenceId;
                            inboxItem.Action = "Creado";
                            inboxItem.CreateAudit(currentUser2Name);
                        }
                        _inboxItemsRepository.Insert(inboxItem);
                        await _unitOfWork.CommitAsync();
                    }
                }
                catch (Exception exInbox)
                {
                    _logger.LogError($"Error creando InboxItem: {exInbox.Message}");
                    // No fallamos la creación del PeriodAudit si falla el inbox
                }
                response.Data = _mapper.Map<PeriodAuditResponseDto>(entityCreate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el registro.");
                    return response;
                }
                entity.IsActive = false;
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<PeriodAuditResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditId == id && x.IsActive,
                    includeProperties: [x => x.Store.Enterprise, x => x.Administrator, x => x.Assistant, x => x.OperationManager, x => x.FloatingAdministrator, x => x.ResponsibleAuditor, x => x.AuditStatus, su => su.Supervisor]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditResponseDto>("No se encontró el registro.");
                    return response;
                }
                response.Data = _mapper.Map<PeriodAuditResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditResponseDto>> Update(Guid id, PeriodAuditRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditId == id && x.IsActive,
                    includeProperties: [x => x.Store.Enterprise, x => x.Administrator, x => x.Assistant, x => x.OperationManager, x => x.FloatingAdministrator, x => x.ResponsibleAuditor, x => x.AuditStatus, su => su.Supervisor]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<PeriodAuditResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<PeriodAuditResponseDto>>> GetPaged(PeriodAuditFilterRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<PeriodAuditResponseDto>>();
            try
            {
                Expression<Func<PeriodAudit, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = filter.AndAlso(x => x.GlobalObservations.Contains(paginationRequestDto.Filter) && x.IsActive);

                if (paginationRequestDto.StoreId.HasValue)
                    filter = filter.AndAlso(x => x.StoreId == paginationRequestDto.StoreId.Value && x.IsActive);

                if (paginationRequestDto.EnterpriseId.HasValue)
                    filter = filter.AndAlso(x => x.Store.EnterpriseId == paginationRequestDto.EnterpriseId.Value && x.IsActive);

                if (paginationRequestDto.ResponsibleAuditorId.HasValue)
                    filter = filter.AndAlso(x => x.ResponsibleAuditorId == paginationRequestDto.ResponsibleAuditorId.Value && x.IsActive);

                if (paginationRequestDto.EndDate.HasValue)
                    filter = filter.AndAlso(x => x.EndDate >= paginationRequestDto.EndDate.Value && x.IsActive);

                if (paginationRequestDto.StartDate.HasValue)
                    filter = filter.AndAlso(x => x.StartDate == paginationRequestDto.StartDate.Value && x.IsActive);

                Func<IQueryable<PeriodAudit>, IOrderedQueryable<PeriodAudit>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _repository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize,
                    includeProperties: [x => x.Store.Enterprise, x => x.Administrator, x => x.Assistant, x => x.OperationManager, x => x.FloatingAdministrator, x => x.ResponsibleAuditor, x => x.AuditStatus, su => su.Supervisor]
                );

                var pagedResult = new PaginationResponseDto<PeriodAuditResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<PeriodAuditResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<PeriodAuditResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<bool>> Recalculate(Guid periodAuditId)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(x => x.PeriodAuditId == periodAuditId && x.IsActive, includeProperties: [x => x.Store]);
                if (entity == null)
                {
                    response = ResponseDto.Error<bool>("No se encontró el registro.");
                    return response;
                }
                var periodAuditGroupResults = await _periodAuditGroupResultRepository.GetByPeriodAuditIdAsync(periodAuditId);
                decimal acumulatedScore = 0;
                foreach (var groupResult in periodAuditGroupResults)
                {
                    var score = groupResult.ScoreValue * (groupResult.TotalWeighting / 100);
                    acumulatedScore += score;
                }
                var scaleCompany = await _scaleCompanyRepository.GetByEnterpriseIdAsync(entity.Store!.EnterpriseId);
                if (scaleCompany == null)
                {
                    response = ResponseDto.Error<bool>($"No se encontró la escala asociada a la empresa.");
                    return response;
                }

                bool scaleFound = false;
                foreach (var scale in scaleCompany)
                {
                    if (acumulatedScore >= scale.MinValue && acumulatedScore <= scale.MaxValue)
                    {
                        entity.ScaleName = scale.Name;
                        entity.ScaleIcon = scale.Icon;
                        entity.ScaleColor = scale.ColorCode;
                        scaleFound = true;
                        break;
                    }
                }
                if (!scaleFound)
                {
                    response = ResponseDto.Error<bool>("No se encontró una escala válida para la puntuación calculada.");
                    return response;
                }

                entity.ScoreValue = acumulatedScore;
                _repository.Update(entity);

                await _unitOfWork.CommitAsync();
                response.Data = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> ProcessAction(Guid periodAuditId, Rokys.Audit.DTOs.Requests.PeriodAudit.PeriodAuditActionRequestDto requestDto)
        {
            var response = ResponseDto.Create();
            try
            {
                // Load period audit with related users and status
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == periodAuditId && x.IsActive,
                    includeProperties: [v => v.Administrator, x => x.ResponsibleAuditor, y => y.AuditStatus]);
                if (entity == null)
                {
                    return ResponseDto.Error("No se encontró el registro.");
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                if (currentUser == null)
                    return ResponseDto.Error("Usuario no autenticado.");
                var currentUserName = currentUser?.UserName ?? "system";

                // Resolve status codes to ids
                var statusPending = await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.Pending && f.IsActive);
                var statusInProgress = await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.InProgress && f.IsActive);
                var statusInReview = await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.InReview && f.IsActive);
                var statusFinal = await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.Completed && f.IsActive);
                var statusCanceled = await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.Canceled && f.IsActive);

                // Normalise action
                var action = requestDto.Action?.Trim().ToLowerInvariant();

                // We'll create an inbox record for every state transition
                var inboxItem = new InboxItems
                {
                    PeriodAuditId = entity.PeriodAuditId,
                    PrevStatusId = entity.StatusId,
                    IsActive = true,
                    Comments = requestDto.Comments
                };

                // Default next user/status
                Guid? nextUserId = null;
                Guid? prevUserId = null;
                _logger.LogDebug("ProcessAction for PeriodAuditId={PeriodAuditId} AdministratorId={AdministratorId} ResponsibleAuditorId={ResponsibleAuditorId} StatusId={StatusId}", entity.PeriodAuditId, entity.AdministratorId, entity.ResponsibleAuditorId, entity.StatusId);
                // PrevUser: if exists assign current ResponsibleAuditor or Administrator depending on who had it
                if (entity.ResponsibleAuditorId.HasValue)
                {
                    var auditorRef = await _userReferenceRepository.GetByUserIdAsync(entity.ResponsibleAuditorId.Value);
                    _logger.LogDebug("Found auditorRef for ResponsibleAuditorId={ResponsibleAuditorId}: {UserReference}", entity.ResponsibleAuditorId, auditorRef != null ? auditorRef.UserReferenceId.ToString() : "null");
                    if (auditorRef != null) prevUserId = auditorRef.UserReferenceId;
                }
                if (prevUserId == null && entity.AdministratorId.HasValue)
                {
                    var adminRef = await _userReferenceRepository.GetByUserIdAsync(entity.AdministratorId.Value);
                    _logger.LogDebug("Found adminRef for AdministratorId={AdministratorId}: {UserReference}", entity.AdministratorId, adminRef != null ? adminRef.UserReferenceId.ToString() : "null");
                    if (adminRef != null) prevUserId = adminRef.UserReferenceId;
                }

                inboxItem.PrevUserId = prevUserId;

                // Handle actions
                if (action == "approve")
                {
                    // move along the chain: Pending -> InProgress -> InReview -> Finalized
                    if (entity.StatusId == statusPending?.AuditStatusId)
                    {
                        entity.StatusId = statusInProgress?.AuditStatusId ?? entity.StatusId;
                        inboxItem.NextStatusId = statusInProgress?.AuditStatusId;
                        // Next user should be responsible auditor
                        if (entity.ResponsibleAuditorId.HasValue)
                        {
                            var auditorRef = await _userReferenceRepository.GetByUserIdAsync(entity.ResponsibleAuditorId.Value);
                            _logger.LogDebug("Found auditorRef for Next ResponsibleAuditorId={ResponsibleAuditorId}: {UserReference}", entity.ResponsibleAuditorId, auditorRef != null ? auditorRef.UserReferenceId.ToString() : "null");
                            if (auditorRef != null) nextUserId = auditorRef.UserReferenceId;
                        }
                    }
                    else if (entity.StatusId == statusInProgress?.AuditStatusId)
                    {
                        entity.StatusId = statusInReview?.AuditStatusId ?? entity.StatusId;
                        inboxItem.NextStatusId = statusInReview?.AuditStatusId;
                        // Next user: maybe a reviewer or supervisor; fallback to administrator
                        if (entity.AdministratorId.HasValue)
                        {
                            var adminRef = await _userReferenceRepository.GetByUserIdAsync(entity.AdministratorId.Value);
                            _logger.LogDebug("Found adminRef for Next AdministratorId={AdministratorId}: {UserReference}", entity.AdministratorId, adminRef != null ? adminRef.UserReferenceId.ToString() : "null");
                            if (adminRef != null) nextUserId = adminRef.UserReferenceId;
                        }
                    }
                    else if (entity.StatusId == statusInReview?.AuditStatusId)
                    {
                        entity.StatusId = statusFinal?.AuditStatusId ?? entity.StatusId;
                        inboxItem.NextStatusId = statusFinal?.AuditStatusId;
                        // Finalized - no next user
                        nextUserId = null;
                    }
                    else
                    {
                        return ResponseDto.Error("La acción de aprobación no aplica en el estado actual.");
                    }

                    inboxItem.Action = "Aprobada";
                }
                else if (action == "cancel" || action == "cancelar")
                {
                    // Can cancel from InProgress (and perhaps Pending)
                    if (entity.StatusId == statusInProgress?.AuditStatusId || entity.StatusId == statusPending?.AuditStatusId)
                    {
                        entity.StatusId = statusCanceled?.AuditStatusId ?? entity.StatusId;
                        inboxItem.NextStatusId = statusCanceled?.AuditStatusId;
                        // canceled -> no next user
                        nextUserId = null;
                        inboxItem.Action = "Cancelada";
                    }
                    else
                    {
                        return ResponseDto.Error("La acción de cancelar no aplica en el estado actual.");
                    }
                }
                else if (action == "return")
                {
                    // Return from InReview -> InProgress and send back to auditor
                    if (entity.StatusId == statusInReview?.AuditStatusId)
                    {
                        entity.StatusId = statusInProgress?.AuditStatusId ?? entity.StatusId;
                        inboxItem.NextStatusId = statusInProgress?.AuditStatusId;
                        // send back to responsible auditor
                        if (entity.ResponsibleAuditorId.HasValue)
                        {
                            var auditorRef = await _userReferenceRepository.GetByUserIdAsync(entity.ResponsibleAuditorId.Value);
                            if (auditorRef != null) nextUserId = auditorRef.UserReferenceId;
                        }
                        inboxItem.Action = "Devuelta";
                    }
                    else
                    {
                        return ResponseDto.Error("La acción de devolver no aplica en el estado actual.");
                    }
                }
                else
                {
                    return ResponseDto.Error("Acción no válida.");
                }

                // Assign next user
                inboxItem.NextUserId = nextUserId;

                // record actor and create audit (map to UserReferenceId)
                if (currentUser != null)
                {
                    var actorRef = await _userReferenceRepository.GetByUserIdAsync(currentUser.UserId);
                    if (actorRef != null)
                        inboxItem.UserId = actorRef.UserReferenceId;
                    inboxItem.CreateAudit(currentUserName);
                }

                // record who made the change on the period audit
                entity.UpdateAudit(currentUserName);
                // Persist changes
                _repository.Update(entity);
                _inboxItemsRepository.Insert(inboxItem);
                await _unitOfWork.CommitAsync();

                response = ResponseDto.Create(new ApplicationMessage[] { });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }
    }
}
