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
    private readonly IInboxItemsService _inboxItemsService;
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
            IInboxItemsService inboxItemsService,
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
            _inboxItemsService = inboxItemsService;
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
                    // Build InboxItemRequestDto and reuse the inbox service to handle creation (sequence number, user mapping, audit fields)
                    var inboxDto = new Rokys.Audit.DTOs.Requests.InboxItems.InboxItemRequestDto
                    {
                        PeriodAuditId = entity.PeriodAuditId,
                        PrevStatusId = (await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.Pending && f.IsActive))?.AuditStatusId,
                        NextStatusId = (await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.InProgress && f.IsActive))?.AuditStatusId,
                        Comments = "Auditoría creada",
                        Action = "Creado",
                        IsActive = true
                    };

                    // set prev/next users using system user ids -> inbox service will map to UserReference internally
                    if (entity.AdministratorId.HasValue)
                        inboxDto.PrevUserId = entity.AdministratorId;
                    if (entity.ResponsibleAuditorId.HasValue)
                        inboxDto.NextUserId = entity.ResponsibleAuditorId;

                    // let the inbox service decide the SequenceNumber and actor mapping
                    var inboxCreateResponse = await _inboxItemsService.Create(inboxDto);
                    if (!inboxCreateResponse.IsValid)
                        _logger.LogWarning("Inbox item creation returned non-valid: {Messages}", string.Join(';', inboxCreateResponse.Messages.Select(m => m.Message)));
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

                if (paginationRequestDto.StartDate.HasValue && paginationRequestDto.EndDate.HasValue)
                    filter = filter.AndAlso(x => x.StartDate >= paginationRequestDto.StartDate.Value && x.EndDate <= paginationRequestDto.EndDate.Value && x.IsActive);

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
                Guid? prevUserId = null;
                Guid? nextUserId = null;
                Guid? prevStatusId = entity.StatusId;
                Guid? nextStatusId = null;
                var comments = requestDto.Comments;
                var actionText = string.Empty;
                // PrevUser: if exists assign current ResponsibleAuditor or Administrator depending on who had it
                if (entity.ResponsibleAuditorId.HasValue)
                {
                    prevUserId = entity.ResponsibleAuditorId.Value;
                    var auditorRef = await _userReferenceRepository.GetFirstOrDefaultAsync(filter: x=>x.UserReferenceId == entity.ResponsibleAuditorId.Value && x.IsActive);
                    if (auditorRef != null) prevUserId = auditorRef.UserReferenceId;
                }
                if (prevUserId == null && entity.AdministratorId.HasValue)
                {
                    var adminRef = await _userReferenceRepository.GetFirstOrDefaultAsync(filter: x => x.UserReferenceId == entity.AdministratorId.Value && x.IsActive);
                    if (adminRef != null) prevUserId = adminRef.UserReferenceId;
                }

                Guid newStatusId;
                // Handle actions
                if (action == "approve")
                {
                    // move along the chain: Pending -> InProgress -> InReview -> Finalized
                    if (entity.StatusId == statusPending?.AuditStatusId)
                    {
                        newStatusId = statusInProgress?.AuditStatusId ?? entity.StatusId ?? Guid.Empty;

                        nextStatusId = statusInProgress?.AuditStatusId;
                        // Next user should be responsible auditor
                        if (entity.ResponsibleAuditorId.HasValue)
                        {
                            var auditorRef = await _userReferenceRepository.GetFirstOrDefaultAsync(filter: x => x.UserReferenceId == entity.ResponsibleAuditorId.Value && x.IsActive);
                            if (auditorRef != null) nextUserId = auditorRef.UserReferenceId;
                        }
                    }
                    else if (entity.StatusId == statusInProgress?.AuditStatusId)
                    {
                        newStatusId = statusInReview?.AuditStatusId ?? entity.StatusId ?? Guid.Empty;
                        nextStatusId = statusInReview?.AuditStatusId;
                        // Next user: maybe a reviewer or supervisor; fallback to administrator
                        if (entity.AdministratorId.HasValue)
                        {
                            var adminRef = await _userReferenceRepository.GetFirstOrDefaultAsync(filter: x => x.UserReferenceId == entity.AdministratorId.Value && x.IsActive);
                            if (adminRef != null) nextUserId = adminRef.UserReferenceId;
                        }
                    }
                    else if (entity.StatusId == statusInReview?.AuditStatusId)
                    {
                        newStatusId = statusFinal?.AuditStatusId ?? entity.StatusId ?? Guid.Empty;
                        nextStatusId = statusFinal?.AuditStatusId;
                        // Finalized - no next user
                        nextUserId = null;
                    }
                    else
                    {
                        return ResponseDto.Error("La acción de aprobación no aplica en el estado actual.");
                    }

                    actionText = "Aprobada";
                }
                else if (action == "cancel" || action == "cancelar")
                {
                    // Can cancel from InProgress (and perhaps Pending)
                    if (entity.StatusId == statusInProgress?.AuditStatusId || entity.StatusId == statusPending?.AuditStatusId)
                    {
                        newStatusId = statusCanceled?.AuditStatusId ?? entity.StatusId ?? Guid.Empty;
                        nextStatusId = statusCanceled?.AuditStatusId;
                        // canceled -> no next user
                        nextUserId = null;
                        actionText = "Cancelada";
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
                        newStatusId = statusInProgress?.AuditStatusId ?? entity.StatusId ?? Guid.Empty;
                        nextStatusId = statusInProgress?.AuditStatusId;
                        // send back to responsible auditor
                        if (entity.ResponsibleAuditorId.HasValue)
                        {
                            var auditorRef = await _userReferenceRepository.GetByUserIdAsync(entity.ResponsibleAuditorId.Value);
                            if (auditorRef != null) nextUserId = auditorRef.UserReferenceId;
                        }
                        actionText = "Devuelta";
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
                // We'll delegate creation of the inbox item to the inbox service (it will calculate sequence and map actor user)
                var inboxDto = new Rokys.Audit.DTOs.Requests.InboxItems.InboxItemRequestDto
                {
                    PeriodAuditId = entity.PeriodAuditId,
                    PrevUserId = prevUserId,
                    NextUserId = nextUserId,
                    PrevStatusId = prevStatusId,
                    NextStatusId = nextStatusId,
                    Comments = comments,
                    Action = actionText,
                    IsActive = true,
                    // actor (system user id) will be mapped by the inbox service to UserReferenceId
                    UserId = currentUser?.UserId
                };

                // Update period audit status
                var periodAudit = await _repository.GetFirstOrDefaultAsync(f => f.PeriodAuditId == entity.PeriodAuditId && f.IsActive);
                periodAudit.StatusId = newStatusId;
                periodAudit.UpdateAudit(currentUserName);

                _repository.Update(periodAudit);

                // Create inbox via service (handles sequence and user mapping)
                var inboxCreateResponse = await _inboxItemsService.Create(inboxDto);
                if (!inboxCreateResponse.IsValid)
                {
                    _logger.LogWarning("Inbox item creation during ProcessAction returned non-valid: {Messages}", string.Join(';', inboxCreateResponse.Messages.Select(m => m.Message)));
                }

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

        public async Task<ResponseDto<LastAuditByStoreIdResponseDto>> GetLasAuditByStoreId(Guid storeId)
        {
            var response = ResponseDto.Create<LastAuditByStoreIdResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.StoreId == storeId && x.IsActive,
                    orderBy: q => q.OrderByDescending(x => x.EndDate)
                );
                if (entity == null)
                {
                    // return empty result instead of null to satisfy nullable reference types
                    response.Data = new LastAuditByStoreIdResponseDto { LastAuditDate = null };
                    return response;
                }
                var lastAudit = new LastAuditByStoreIdResponseDto
                {
                    LastAuditDate = entity?.EndDate
                };
                response.Data = lastAudit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<LastAuditByStoreIdResponseDto>(ex.Message);
            }
            return response;
        }

    }
}
