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
        private readonly IPeriodAuditScaleResultRepository _periodAuditScaleResultRepository;
        private readonly IPeriodAuditTableScaleTemplateResultRepository _periodAuditTableScaleTemplateResultRepository;
        private readonly IPeriodAuditFieldValuesRepository _periodAuditFieldValuesRepository;
        private readonly IPeriodAuditScaleSubResultRepository _periodAuditScaleSubResultRepository;
        private readonly IPeriodAuditScoringCriteriaResultRepository _periodAuditScoringCriteriaResultRepository;

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
            IUserReferenceRepository userReferenceRepository,
            IPeriodAuditScaleResultRepository periodAuditScaleResultRepository,
            IPeriodAuditTableScaleTemplateResultRepository periodAuditTableScaleTemplateResultRepository,
            IPeriodAuditFieldValuesRepository periodAuditFieldValuesRepository,
            IPeriodAuditScaleSubResultRepository periodAuditScaleSubResultRepository,
            IPeriodAuditScoringCriteriaResultRepository periodAuditScoringCriteriaResultRepository)
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
            _periodAuditScaleResultRepository = periodAuditScaleResultRepository;
            _periodAuditTableScaleTemplateResultRepository = periodAuditTableScaleTemplateResultRepository;
            _periodAuditFieldValuesRepository = periodAuditFieldValuesRepository;
            _periodAuditScaleSubResultRepository = periodAuditScaleSubResultRepository;
            _periodAuditScoringCriteriaResultRepository = periodAuditScoringCriteriaResultRepository;
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
                var nextCode = CodeGeneratorHelper.GenerateNextCode("AUD" + currentYear, lastCode, 4);
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
                    //var currentUserReference = await _userReferenceRepository.GetByUserIdAsync(currentUser?.UserId ?? Guid.Empty);
                    //if (currentUserReference != null) 
                    //{
                    //    response.Messages.Add(new ApplicationMessage
                    //    {
                    //        Message = "No se encontro al usuario que esta creando",
                    //        MessageType = ApplicationMessageType.Error,
                    //    });
                    //}
                    var inboxDto = new Rokys.Audit.DTOs.Requests.InboxItems.InboxItemRequestDto
                    {
                        PeriodAuditId = entity.PeriodAuditId,
                        PrevStatusId = (await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.Pending && f.IsActive))?.AuditStatusId,
                        NextStatusId = (await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.InProgress && f.IsActive))?.AuditStatusId,
                        //UserId = currentUserReference?.UserReferenceId ?? null,
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
                var periodAuditGroupResults = await _periodAuditGroupResultRepository.GetAsync(filter: x => x.PeriodAuditId == id && x.IsActive);
                foreach (var groupResult in periodAuditGroupResults)
                {
                    var scaleResults = await _periodAuditScaleResultRepository.GetAsync(x => x.PeriodAuditGroupResultId == groupResult.PeriodAuditGroupResultId && x.IsActive);
                    foreach (var scaleResult in scaleResults)
                    {
                        var tableResults = await _periodAuditTableScaleTemplateResultRepository.GetAsync(x => x.PeriodAuditScaleResultId == scaleResult.PeriodAuditScaleResultId && x.IsActive);
                        foreach (var tableResult in tableResults)
                        {
                            var fieldValues = await _periodAuditFieldValuesRepository.GetAsync(x => x.PeriodAuditTableScaleTemplateResultId == tableResult.PeriodAuditTableScaleTemplateResultId && x.IsActive);
                            foreach (var fieldValue in fieldValues)
                            {
                                fieldValue.IsActive = false;
                                fieldValue.UpdateDate = DateTime.Now;
                                _periodAuditFieldValuesRepository.Update(fieldValue);
                            }

                            tableResult.IsActive = false;
                            tableResult.UpdateDate = DateTime.Now;
                            _periodAuditTableScaleTemplateResultRepository.Update(tableResult);
                        }

                        var subResults = await _periodAuditScaleSubResultRepository.GetAsync(x => x.PeriodAuditScaleResultId == scaleResult.PeriodAuditScaleResultId && x.IsActive);
                        foreach (var subResult in subResults)
                        {
                            subResult.IsActive = false;
                            subResult.UpdateDate = DateTime.Now;
                            _periodAuditScaleSubResultRepository.Update(subResult);
                        }

                        var scoringResults = await _periodAuditScoringCriteriaResultRepository.GetAsync(x => x.PeriodAuditScaleResultId == scaleResult.PeriodAuditScaleResultId && x.IsActive);
                        foreach (var scoringResult in scoringResults)
                        {
                            scoringResult.IsActive = false;
                            scoringResult.UpdateDate = DateTime.Now;
                            _periodAuditScoringCriteriaResultRepository.Update(scoringResult);
                        }

                        scaleResult.IsActive = false;
                        scaleResult.UpdateDate = DateTime.Now;
                        _periodAuditScaleResultRepository.Update(scaleResult);
                    }
                    groupResult.IsActive = false;
                    _periodAuditGroupResultRepository.Update(groupResult);
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
                // Load related inbox items and map them
                var inboxEntities = await _inboxItemsRepository.GetAsync(filter: x => x.PeriodAuditId == entity.PeriodAuditId && x.IsActive, orderBy: q => q.OrderBy(s => s.SequenceNumber));
                var inboxDtos = _mapper.Map<IEnumerable<Rokys.Audit.DTOs.Responses.InboxItems.InboxItemResponseDto>>(inboxEntities ?? new List<InboxItems>());
                response.Data!.InboxItems = inboxDtos.ToList();
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
                    filter = filter.AndAlso(x => x.CreationDate >= paginationRequestDto.StartDate.Value && x.CreationDate <= paginationRequestDto.EndDate.Value && x.IsActive);

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

                // For the paged items, load inbox items in batch and attach them to each response DTO
                var itemsList = pagedResult.Items.ToList();
                var periodIds = entities.Items.Select(i => i.PeriodAuditId).ToList();
                var inboxForPeriods = await _inboxItemsRepository.GetAsync(filter: x => periodIds.Contains(x.PeriodAuditId!.Value) && x.IsActive);
                var inboxMapped = _mapper.Map<IEnumerable<Rokys.Audit.DTOs.Responses.InboxItems.InboxItemResponseDto>>(inboxForPeriods ?? new List<InboxItems>());
                var inboxLookup = inboxMapped.GroupBy(i => i.PeriodAuditId ?? Guid.Empty).ToDictionary(g => g.Key, g => g.OrderBy(x => x.SequenceNumber).ToList());
                foreach (var it in itemsList)
                {
                    var key = it.PeriodAuditId;
                    if (key != Guid.Empty && inboxLookup.TryGetValue(key, out var list))
                        it.InboxItems = list;
                }

                pagedResult.Items = itemsList;
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
                        entity.ScaleCode = scale.Code;
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

        public async Task<ResponseDto> ProcessAction(Rokys.Audit.DTOs.Requests.PeriodAudit.PeriodAuditBatchActionRequestDto requestDto)
        {
            var response = ResponseDto.Create();
            try
            {
                var ids = requestDto.PeriodAuditIds?.Distinct().ToList() ?? new List<Guid>();
                if (!ids.Any())
                    return ResponseDto.Error("No se proporcionaron PeriodAuditIds.");

                var action = requestDto.Action?.Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(action))
                    return ResponseDto.Error("Acción no válida.");

                var currentUser = _httpContextAccessor.CurrentUser();
                if (currentUser == null)
                    return ResponseDto.Error("Usuario no autenticado.");
                var currentUserName = currentUser?.UserName ?? "system";

                // Preload status codes once
                var statusPending = await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.Pending && f.IsActive);
                var statusInProgress = await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.InProgress && f.IsActive);
                var statusInReview = await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.InReview && f.IsActive);
                var statusFinal = await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.Completed && f.IsActive);
                var statusCanceled = await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.Canceled && f.IsActive);

                // Load all period audits
                var entities = new List<PeriodAudit>();
                foreach (var id in ids)
                {
                    var ent = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == id && x.IsActive,
                        includeProperties: [v => v.Administrator, x => x.ResponsibleAuditor, y => y.AuditStatus]);
                    if (ent == null)
                        return ResponseDto.Error($"No se encontró el registro PeriodAuditId={id}");
                    entities.Add(ent);
                }

                // Validate that the action can be applied to all entities
                var invalids = new List<string>();
                foreach (var ent in entities)
                {
                    bool ok = action switch
                    {
                        "approve" => ent.StatusId == statusPending?.AuditStatusId || ent.StatusId == statusInProgress?.AuditStatusId || ent.StatusId == statusInReview?.AuditStatusId,
                        "cancel" => ent.StatusId == statusInProgress?.AuditStatusId || ent.StatusId == statusPending?.AuditStatusId,
                        "return" => ent.StatusId == statusInReview?.AuditStatusId,
                        _ => false
                    };
                    if (!ok) invalids.Add(ent.PeriodAuditId.ToString());
                }
                if (invalids.Any())
                    return ResponseDto.Error($"La acción no es aplicable a los siguientes PeriodAuditIds: {string.Join(',', invalids)}");

                // Additional validation: Check for ScaleName requirement BEFORE making any changes
                if (action == "approve")
                {
                    foreach (var ent in entities)
                    {
                        if (ent.StatusId == statusInProgress?.AuditStatusId && string.IsNullOrEmpty(ent.ScaleName))
                        {
                            return ResponseDto.Error($"No se puede aprobar para revisión la auditoría {ent.CorrelativeNumber} sin puntuación.");
                        }
                    }
                }

                // All validations passed - process each entity (one transaction)
                foreach (var ent in entities)
                {
                    // reuse the previous behavior for a single entity: compute nextStatusId, nextUserId, prevUserId, actionText
                    // (extract core logic inline to avoid duplicating heavy refactor)
                    // Prev user
                    Guid? prevUserId = null;
                    Guid? nextUserId = null;
                    if (ent.ResponsibleAuditorId.HasValue)
                    {
                        var auditorRef = await _userReferenceRepository.GetFirstOrDefaultAsync(f => f.UserReferenceId == ent.ResponsibleAuditorId.Value && f.IsActive);
                        if (auditorRef != null) prevUserId = auditorRef.UserReferenceId;
                    }
                    if (prevUserId == null && ent.AdministratorId.HasValue)
                    {
                        var adminRef = await _userReferenceRepository.GetFirstOrDefaultAsync(f => f.UserReferenceId == ent.AdministratorId.Value && f.IsActive);
                        if (adminRef != null) prevUserId = adminRef.UserReferenceId;
                    }

                    Guid? nextStatusId = null;
                    Guid newStatusId;
                    string actionText = string.Empty;
                    if (action == "approve")
                    {
                        if (ent.StatusId == statusPending?.AuditStatusId)
                        {
                            newStatusId = statusInProgress?.AuditStatusId ?? ent.StatusId ?? Guid.Empty;
                            nextStatusId = statusInProgress?.AuditStatusId;
                            if (ent.ResponsibleAuditorId.HasValue)
                            {
                                var auditorRef = await _userReferenceRepository.GetFirstOrDefaultAsync(f => f.UserReferenceId == ent.ResponsibleAuditorId.Value && f.IsActive);
                                if (auditorRef != null) nextUserId = auditorRef.UserReferenceId;
                            }
                        }
                        else if (ent.StatusId == statusInProgress?.AuditStatusId)
                        {
                            newStatusId = statusInReview?.AuditStatusId ?? ent.StatusId ?? Guid.Empty;
                            nextStatusId = statusInReview?.AuditStatusId;
                            // ScaleName validation already done above before any changes
                            if (ent.AdministratorId.HasValue)
                            {
                                var adminRef = await _userReferenceRepository.GetFirstOrDefaultAsync(f => f.UserReferenceId == ent.AdministratorId.Value && f.IsActive);
                                if (adminRef != null) nextUserId = adminRef.UserReferenceId;
                            }
                        }
                        else // in review -> finalize
                        {
                            newStatusId = statusFinal?.AuditStatusId ?? ent.StatusId ?? Guid.Empty;
                            nextStatusId = statusFinal?.AuditStatusId;
                            nextUserId = null;
                        }
                        actionText = "Aprobada";
                    }
                    else if (action == "cancel" || action == "cancelar")
                    {
                        newStatusId = statusCanceled?.AuditStatusId ?? ent.StatusId ?? Guid.Empty;
                        nextStatusId = statusCanceled?.AuditStatusId;
                        nextUserId = null;
                        actionText = "Cancelada";
                    }
                    else if (action == "return")
                    {
                        newStatusId = statusInProgress?.AuditStatusId ?? ent.StatusId ?? Guid.Empty;
                        nextStatusId = statusInProgress?.AuditStatusId;
                        if (ent.ResponsibleAuditorId.HasValue)
                        {
                            var auditorRef = await _userReferenceRepository.GetByUserIdAsync(ent.ResponsibleAuditorId.Value);
                            if (auditorRef != null) nextUserId = auditorRef.UserReferenceId;
                        }
                        actionText = "Devuelta";
                    }
                    else
                    {
                        return ResponseDto.Error("Acción no válida.");
                    }

                    // Build inbox dto and update period audit status
                    var inboxDto = new Rokys.Audit.DTOs.Requests.InboxItems.InboxItemRequestDto
                    {
                        PeriodAuditId = ent.PeriodAuditId,
                        PrevUserId = prevUserId,
                        NextUserId = nextUserId,
                        PrevStatusId = ent.StatusId,
                        NextStatusId = nextStatusId,
                        Comments = requestDto.Comments,
                        Action = actionText,
                        IsActive = true,
                        UserId = currentUser?.UserId
                    };

                    // Update period audit entity
                    var periodAudit = await _repository.GetFirstOrDefaultAsync(f => f.PeriodAuditId == ent.PeriodAuditId && f.IsActive);
                    periodAudit.StatusId = newStatusId;
                    periodAudit.UpdateAudit(currentUserName);
                    _repository.Update(periodAudit);

                    var inboxCreateResponse = await _inboxItemsService.Create(inboxDto);
                    if (!inboxCreateResponse.IsValid)
                        _logger.LogWarning("Inbox item creation during batch ProcessAction returned non-valid for {Id}: {Messages}", ent.PeriodAuditId, string.Join(';', inboxCreateResponse.Messages.Select(m => m.Message)));
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
