using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Vml.Office;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.Common.Helpers;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.PeriodAudit;
using Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.PeriodAudit;
using Rokys.Audit.External.Services;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Services.Services.Emails;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class PeriodAuditService : IPeriodAuditService
    {
        private readonly IPeriodAuditRepository _periodAuditRepository;
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
        private readonly IGroupRepository _groupRepository;
        private readonly IEnterpriseRepository _enterpriseRepository;
        private readonly IPeriodAuditGroupResultService _periodAuditGroupResultService;
        private readonly IStoreRepository _storeRepository;
        private readonly IEmailService _emailService;
        private readonly IPeriodAuditParticipantRepository _periodAuditParticipantRepository;

        public PeriodAuditService(
            IPeriodAuditRepository periodAuditRepository,
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
            IPeriodAuditScoringCriteriaResultRepository periodAuditScoringCriteriaResultRepository,
            IGroupRepository groupRepository,
            IPeriodAuditGroupResultService periodAuditGroupResultService,
            IStoreRepository storeRepository,
            IEmailService emailService,
            IPeriodAuditParticipantRepository periodAuditParticipantRepository)
        {
            _periodAuditRepository = periodAuditRepository;
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
            _groupRepository = groupRepository;
            _periodAuditGroupResultService = periodAuditGroupResultService;
            _storeRepository = storeRepository;
            _emailService = emailService;
            _periodAuditParticipantRepository = periodAuditParticipantRepository;
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
                var lastCode = _periodAuditRepository.Get()
                    .OrderByDescending(x => x.CreationDate)
                    .Select(x => x.CorrelativeNumber)
                    .FirstOrDefault();
                var currentYear = DateTime.Now.Year;
                var nextCode = CodeGeneratorHelper.GenerateNextCode("AUD" + currentYear, lastCode, 4);
                entity.CorrelativeNumber = nextCode;
                entity.StatusId = auditStatus.AuditStatusId;
                entity.CreateAudit(currentUserName);
                foreach (var detail in requestDto.Participants)
                {
                    var newParticipant = _mapper.Map<PeriodAuditParticipant>(detail);
                    newParticipant.CreateAudit(currentUserName);
                    newParticipant.IsActive = true;
                    entity.PeriodAuditParticipants.Add(newParticipant);
                }
                _periodAuditRepository.Insert(entity);

                var store = await _storeRepository.GetFirstOrDefaultAsync(filter: x => x.StoreId == entity.StoreId && x.IsActive);
                var group = await _groupRepository.GetAsync(filter: x => x.EnterpriseId == store.EnterpriseId && x.IsActive);
                // Crear resultados de grupo de auditoría asociados a la nueva auditoría
                foreach (var grp in group)
                {
                    var newPeriodAuditGroupResult = new PeriodAuditGroupResultRequestDto
                    {
                        PeriodAuditId = entity.PeriodAuditId,
                        GroupId = grp.GroupId,
                        ScoreValue = 0,
                        TotalWeighting = grp.Weighting,
                    };
                    await _periodAuditGroupResultService.Create(newPeriodAuditGroupResult, true);
                }

                await _unitOfWork.CommitAsync();
                // Crear registro en InboxItems: prev user = administrador, next user = auditor responsable
                try
                {
                    // Build InboxItemRequestDto and reuse the inbox service to handle creation (sequence number, user mapping, audit fields)
                    var currentUserReferenceId = currentUser?.UserReferenceId;
                    if (currentUserReferenceId == null || currentUserReferenceId.Equals(Guid.Empty))
                    {
                        response.Messages.Add(new ApplicationMessage
                        {
                            Message = "No se encontro al usuario que esta creando",
                            MessageType = ApplicationMessageType.Error,
                        });
                    }
                    var inboxDto = new DTOs.Requests.InboxItems.InboxItemRequestDto
                    {
                        PeriodAuditId = entity.PeriodAuditId,
                        PrevStatusId = null,
                        NextStatusId = (await _auditStatusRepository.GetFirstOrDefaultAsync(f => f.Code == AuditStatusCode.Pending && f.IsActive))?.AuditStatusId,
                        UserId = currentUserReferenceId,
                        Comments = "Auditoría creada",
                        Action = "Creado",
                        IsActive = true
                    };
                    inboxDto.PrevUserId = entity.PeriodAuditParticipants.FirstOrDefault(a => a.RoleCodeSnapshot == RoleCodes.JefeDeArea.Code)?.UserReferenceId;
                    inboxDto.NextUserId = entity.PeriodAuditParticipants.FirstOrDefault(a => a.RoleCodeSnapshot == RoleCodes.Auditor.Code)?.UserReferenceId;

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
                response.Data = _mapper.Map<PeriodAuditResponseDto>(entity);
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
                var entity = await _periodAuditRepository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == id && x.IsActive);
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
                _periodAuditRepository.Update(entity);
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
                var entity = await _periodAuditRepository.GetCustomByIdAsync(filter: x => x.PeriodAuditId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditResponseDto>("No se encontró el registro.");
                    return response;
                }
                response.Data = _mapper.Map<PeriodAuditResponseDto>(entity);
                // Load related inbox items and map them
                var inboxEntities = await _inboxItemsRepository.GetAsync(filter: x => x.PeriodAuditId == entity.PeriodAuditId && x.IsActive, includeProperties: [x => x.NextStatus, t => t.User, p => p.PrevStatus], orderBy: q => q.OrderBy(s => s.SequenceNumber));
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

                var entity = await _periodAuditRepository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == id && x.IsActive, includeProperties: [x => x.PeriodAuditParticipants]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditResponseDto>("No se encontró el registro.");
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();

                foreach (var participant in entity.PeriodAuditParticipants)
                {
                    _periodAuditParticipantRepository.Delete(participant);
                }

                foreach (var detail in requestDto.Participants)
                {
                    var newParticipant = _mapper.Map<PeriodAuditParticipant>(detail);
                    newParticipant.CreateAudit(currentUser.UserName);
                    newParticipant.PeriodAuditId = entity.PeriodAuditId;
                    newParticipant.IsActive = true;
                    _periodAuditParticipantRepository.Insert(newParticipant);
                }

                entity.StoreId = requestDto.StoreId;
                entity.EndDate = requestDto.EndDate;
                entity.AuditedDays = requestDto.AuditedDays;
                entity.ReportDate = requestDto.ReportDate;
                entity.StartDate = requestDto.StartDate;

                entity.UpdateAudit(currentUser.UserName);
                _periodAuditRepository.Update(entity);
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
                    filter = filter.AndAlso(x => x.PeriodAuditParticipants.Any(a=> a.UserReferenceId == paginationRequestDto.ResponsibleAuditorId.Value && a.RoleCodeSnapshot == RoleCodes.Auditor.Code) && x.IsActive);

                if (paginationRequestDto.StartDate.HasValue && paginationRequestDto.EndDate.HasValue)
                    filter = filter.AndAlso(x => x.CreationDate >= paginationRequestDto.StartDate.Value && x.CreationDate <= paginationRequestDto.EndDate.Value && x.IsActive);

                if (paginationRequestDto.AuditStatusId.HasValue)
                    filter = filter.AndAlso(x => x.StatusId == paginationRequestDto.AuditStatusId.Value && x.IsActive);

                Func<IQueryable<PeriodAudit>, IOrderedQueryable<PeriodAudit>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _periodAuditRepository.GetSearchPagedAsync(
                    filter: filter,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize
                );
                var pagedResult = new PaginationResponseDto<PeriodAuditResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<PeriodAuditResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };


                var currentUser = _httpContextAccessor.CurrentUser();
                var userReference = await _userReferenceRepository.GetFirstOrDefaultAsync(filter: x => x.UserReferenceId == currentUser.UserReferenceId);
                foreach (var ent in pagedResult.Items)
                {
                    if (ent.Participants.Any(a=>a.UserReferenceId == currentUser.UserReferenceId && a.RoleCodeSnapshot == RoleCodes.Auditor.Code))
                    {
                        ent.IAmAuditor = true;
                    }
                    else
                    {
                        ent.IAmAuditor = false;
                    }
                }

                // For the paged items, load inbox items in batch and attach them to each response DTO

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
                var entity = await _periodAuditRepository.GetFirstOrDefaultAsync(x => x.PeriodAuditId == periodAuditId && x.IsActive, includeProperties: [x => x.Store]);
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
                if (scaleCompany == null || !scaleCompany.Any())
                {
                    scaleCompany = await _scaleCompanyRepository.GetAsync(filter: e => e.EnterpriseId == null);

                    if (scaleCompany == null || !scaleCompany.Any())
                    {
                        response = ResponseDto.Error<bool>("No se encontró la escala asociada a la empresa ni la escala por defecto.");
                        return response;
                    }
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
                _periodAuditRepository.Update(entity);

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

                // Validate that the action is one of the supported actions
                if (action != PeriodAuditAction.Approve && action != PeriodAuditAction.Cancel && action != PeriodAuditAction.Return)
                    return ResponseDto.Error($"Acción '{requestDto.Action}' no es válida. Acciones permitidas: {PeriodAuditAction.Approve}, {PeriodAuditAction.Cancel}, {PeriodAuditAction.Return}.");

                var currentUser = _httpContextAccessor.CurrentUser();
                if (currentUser == null)
                    return ResponseDto.Error("Usuario no autenticado.");
                var currentUserName = currentUser?.UserName ?? "system";

                var allStatus = await _auditStatusRepository.GetAsync(a => a.IsActive);
                // Preload status codes once
                var statusPending = allStatus.FirstOrDefault(f => f.Code == AuditStatusCode.Pending);
                var statusInProgress = allStatus.FirstOrDefault(f => f.Code == AuditStatusCode.InProgress);
                var statusInReview = allStatus.FirstOrDefault(f => f.Code == AuditStatusCode.InReview);
                var statusFinal = allStatus.FirstOrDefault(f => f.Code == AuditStatusCode.Completed);
                var statusCanceled = allStatus.FirstOrDefault(f => f.Code == AuditStatusCode.Canceled);

                // Validate that all required statuses are available
                if (statusPending == null || statusInProgress == null || statusInReview == null || statusFinal == null || statusCanceled == null)
                {
                    return ResponseDto.Error("Los estados de auditoría no están configurados correctamente. Faltan estados requeridos en la base de datos.");
                }

                // Load all period audits
                var entities = new List<PeriodAudit>();
                foreach (var id in ids)
                {
                    var ent = await _periodAuditRepository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == id && x.IsActive,
                        includeProperties: [v => v.PeriodAuditParticipants, y => y.AuditStatus]);
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
                        PeriodAuditAction.Approve => ent.StatusId == statusPending?.AuditStatusId || ent.StatusId == statusInProgress?.AuditStatusId || ent.StatusId == statusInReview?.AuditStatusId,
                        PeriodAuditAction.Cancel => ent.StatusId == statusInProgress?.AuditStatusId || ent.StatusId == statusPending?.AuditStatusId || ent.StatusId == statusInReview?.AuditStatusId,
                        PeriodAuditAction.Return => ent.StatusId == statusInReview?.AuditStatusId,
                        _ => false
                    };
                    if (!ok) invalids.Add(ent.PeriodAuditId.ToString());
                }
                if (invalids.Any())
                    return ResponseDto.Error($"La acción no es aplicable a los siguientes PeriodAuditIds: {string.Join(',', invalids)}");

                // Additional validation: Check for ScaleName requirement BEFORE making any changes
                if (action == PeriodAuditAction.Approve)
                {
                    foreach (var ent in entities)
                    {
                        if (ent.StatusId == statusInProgress?.AuditStatusId && string.IsNullOrEmpty(ent.ScaleName))
                        {
                            return ResponseDto.Error($"No se puede aprobar para revisión la auditoría {ent.CorrelativeNumber} sin puntuación.");
                        }
                        if (ent.StatusId == statusPending?.AuditStatusId)
                        {
                            var periodAuditGroupResults = await _periodAuditGroupResultRepository.GetByPeriodAuditIdAsync(ent.PeriodAuditId);
                            if (!periodAuditGroupResults.Any())
                            {
                                return ResponseDto.Error($"No se puede aprobar para revisión la auditoría {ent.CorrelativeNumber} sin la configuración correspondiente.");
                            }
                        }
                    }
                }

                // All validations passed - process each entity (one transaction)
                foreach (var ent in entities)
                {
                    // reuse the previous behavior for a single entity: compute nextStatusId, nextUserId, prevUserId, actionText
                    // (extract core logic inline to avoid duplicating heavy refactor)

                    // Get PrevUserId from the last inbox item's NextUserId for this audit
                    Guid? prevUserId = null;
                    var lastInboxItem = await _inboxItemsRepository.GetFirstOrDefaultAsync(
                        filter: x => x.PeriodAuditId == ent.PeriodAuditId && x.IsActive,
                        orderBy: q => q.OrderByDescending(x => x.SequenceNumber)
                    );
                    if (lastInboxItem != null)
                    {
                        prevUserId = lastInboxItem.NextUserId;
                    }

                    // Initialize NextUserId - will be set based on the action and target status
                    Guid? nextUserId = null;

                    Guid? nextStatusId = null;
                    Guid? newStatusId = null;
                    string actionText = string.Empty;
                    if (action == PeriodAuditAction.Approve)
                    {
                        if (ent.StatusId == statusPending?.AuditStatusId)
                        {
                            newStatusId = statusInProgress?.AuditStatusId ?? ent.StatusId ?? Guid.Empty;
                            nextStatusId = statusInProgress?.AuditStatusId;
                        
                            nextUserId = ent.PeriodAuditParticipants.FirstOrDefault(a => a.RoleCodeSnapshot == RoleCodes.Auditor.Code)?.UserReferenceId;
                          
                            actionText = "Aprobado";
                        }
                        if (ent.StatusId == statusInProgress?.AuditStatusId)
                        {
                            newStatusId = statusInReview?.AuditStatusId ?? ent.StatusId ?? Guid.Empty;
                            nextStatusId = statusInReview?.AuditStatusId;
                          
                            nextUserId = ent.PeriodAuditParticipants.FirstOrDefault(a => a.RoleCodeSnapshot == RoleCodes.JefeDeArea.Code)?.UserReferenceId;
                          
                            actionText = "Enviado a revisión";
                        }
                        else if (ent.StatusId == statusInReview?.AuditStatusId)
                        {
                            newStatusId = statusFinal?.AuditStatusId ?? ent.StatusId ?? Guid.Empty;
                            nextStatusId = statusFinal?.AuditStatusId;
                            // InReview → Final: NextUser = Administrator (finalizó)
                            nextUserId = ent.PeriodAuditParticipants.FirstOrDefault(a => a.RoleCodeSnapshot == RoleCodes.JefeDeArea.Code)?.UserReferenceId;
                          
                            actionText = "Finalizado";
                        }
                    }
                    else if (action == PeriodAuditAction.Cancel)
                    {
                        newStatusId = statusCanceled?.AuditStatusId ?? ent.StatusId ?? Guid.Empty;
                        nextStatusId = statusCanceled?.AuditStatusId;
                        nextUserId = null;
                        actionText = "Cancelado";
                    }
                    else if (action == PeriodAuditAction.Return)
                    {
                        newStatusId = statusInProgress?.AuditStatusId ?? ent.StatusId ?? Guid.Empty;
                        nextStatusId = statusInProgress?.AuditStatusId;
                        // Return to InProgress: NextUser = Auditor
                        nextUserId = ent.PeriodAuditParticipants.FirstOrDefault(a => a.RoleCodeSnapshot == RoleCodes.Auditor.Code)?.UserReferenceId;
                        actionText = "Devuelto";
                    }

                    // Validate that we have a valid status transition
                    if (newStatusId == null || newStatusId == Guid.Empty)
                    {
                        return ResponseDto.Error($"No se pudo determinar el estado de transición para la auditoría {ent.CorrelativeNumber}. Verifique que los estados de auditoría estén configurados correctamente.");
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
                    var periodAudit = await _periodAuditRepository.GetFirstOrDefaultAsync(f => f.PeriodAuditId == ent.PeriodAuditId && f.IsActive);
                    periodAudit.StatusId = newStatusId;
                    periodAudit.UpdateAudit(currentUserName);
                    _periodAuditRepository.Update(periodAudit);

                    var inboxCreateResponse = await _inboxItemsService.Create(inboxDto);
                    if (!inboxCreateResponse.IsValid)
                        _logger.LogWarning("Inbox item creation during batch ProcessAction returned non-valid for {Id}: {Messages}", ent.PeriodAuditId, string.Join(';', inboxCreateResponse.Messages.Select(m => m.Message)));
                }
                await _unitOfWork.CommitAsync();

                foreach (var ent in entities)
                {
                    var periodAuditUpdate = await _periodAuditRepository.GetCustomByIdAsync(filter: x => x.PeriodAuditId == ent.PeriodAuditId && x.IsActive);
                    if (periodAuditUpdate.StatusId == statusInReview?.AuditStatusId)
                    {
                        await BuildSendEmail.NotifySupervisorOfNewAudit(_emailService, periodAuditUpdate);
                    }
                }

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
                var entity = await _periodAuditRepository.GetFirstOrDefaultAsync(
                    filter: x => x.StoreId == storeId && x.IsActive,
                    orderBy: q => q.OrderByDescending(x => x.ReportDate)
                );
                if (entity == null)
                {
                    // return empty result instead of null to satisfy nullable reference types
                    response.Data = new LastAuditByStoreIdResponseDto { LastAuditDate = null };
                    return response;
                }
                var lastAudit = new LastAuditByStoreIdResponseDto
                {
                    LastAuditDate = entity?.ReportDate
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
