using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Vml.Office;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using ClosedXML.Excel;
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
        private readonly IPeriodAuditActionPlanRepository _periodAuditActionPlanRepository;
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
        private readonly IAuditRoleConfigurationRepository _auditRoleConfigurationRepository;
        private readonly WebAppSettings _webAppSettings;
        private readonly FileSettings _fileSettings;

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
            IPeriodAuditActionPlanRepository periodAuditActionPlanRepository,
            IPeriodAuditTableScaleTemplateResultRepository periodAuditTableScaleTemplateResultRepository,
            IPeriodAuditFieldValuesRepository periodAuditFieldValuesRepository,
            IPeriodAuditScaleSubResultRepository periodAuditScaleSubResultRepository,
            IPeriodAuditScoringCriteriaResultRepository periodAuditScoringCriteriaResultRepository,
            IGroupRepository groupRepository,
            IPeriodAuditGroupResultService periodAuditGroupResultService,
            IStoreRepository storeRepository,
            IEmailService emailService,
            IPeriodAuditParticipantRepository periodAuditParticipantRepository,
            IAuditRoleConfigurationRepository auditRoleConfigurationRepository,
            WebAppSettings webAppSettings,
            FileSettings fileSettings)
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
            _periodAuditActionPlanRepository = periodAuditActionPlanRepository;
            _periodAuditTableScaleTemplateResultRepository = periodAuditTableScaleTemplateResultRepository;
            _periodAuditFieldValuesRepository = periodAuditFieldValuesRepository;
            _periodAuditScaleSubResultRepository = periodAuditScaleSubResultRepository;
            _periodAuditScoringCriteriaResultRepository = periodAuditScoringCriteriaResultRepository;
            _groupRepository = groupRepository;
            _periodAuditGroupResultService = periodAuditGroupResultService;
            _storeRepository = storeRepository;
            _emailService = emailService;
            _periodAuditParticipantRepository = periodAuditParticipantRepository;
            _auditRoleConfigurationRepository = auditRoleConfigurationRepository;
            _webAppSettings = webAppSettings;
            _fileSettings = fileSettings;
        }

        public async Task<ResponseDto<PeriodAuditResponseDto>> Create(PeriodAuditRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditResponseDto>();
            try
            {
                var currentUser = _httpContextAccessor.CurrentUser();
                var currentUserReferenceId = currentUser?.UserReferenceId;
                if (currentUserReferenceId == null || currentUserReferenceId.Equals(Guid.Empty))
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "El usuario que está creando, no tiene referencia en el sistema de auditoría. Ingrese a la aplicación de seguridad y vuelva asignar el usuario " + currentUser?.FullName,
                        MessageType = ApplicationMessageType.Error,
                    });
                }
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                var auditStatus = await _auditStatusRepository.GetFirstOrDefaultAsync(filter: x => x.Code == AuditStatusCode.Pending && x.IsActive);

                var currentUserName = currentUser?.UserName ?? "system";
                var entity = _mapper.Map<PeriodAudit>(requestDto);
                // Obtener el último código existente
                var currentYear = DateTime.Now.Year;
                var lastCode = _periodAuditRepository.Get()
                    .OrderByDescending(x => x.CreationDate)
                    .Select(x => x.CorrelativeNumber)
                    .FirstOrDefault();
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
                var group = await _groupRepository.GetAsync(filter: x => x.EnterpriseId == store.EnterpriseId && x.IsActive,
                                                    orderBy: q => q.OrderBy(x => x.SortOrder));
                // Crear resultados de grupo de auditoría asociados a la nueva auditoría
                foreach (var grp in group)
                {
                    var newPeriodAuditGroupResult = new PeriodAuditGroupResultRequestDto
                    {
                        PeriodAuditId = entity.PeriodAuditId,
                        GroupId = grp.GroupId,
                        ScoreValue = 0,
                        TotalWeighting = grp.Weighting,
                        StartDate = entity.StartDate,
                        EndDate = entity.EndDate
                    };
                    await _periodAuditGroupResultService.Create(newPeriodAuditGroupResult, true);
                }

                await _unitOfWork.CommitAsync();
                // Crear registro en InboxItems: prev user = administrador, next user = auditor responsable
                try
                {
                    // Build InboxItemRequestDto and reuse the inbox service to handle creation (sequence number, user mapping, audit fields)

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
                    inboxDto.UserId = currentUser?.UserReferenceId;

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
                var currentUser = _httpContextAccessor.CurrentUser();
                Expression<Func<PeriodAudit, bool>> filter = x => x.IsActive;


                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = filter.AndAlso(x => x.GlobalObservations.Contains(paginationRequestDto.Filter) && x.IsActive);

                if (paginationRequestDto.StoreId.HasValue)
                    filter = filter.AndAlso(x => x.StoreId == paginationRequestDto.StoreId.Value && x.IsActive);

                if (paginationRequestDto.EnterpriseId.HasValue)
                    filter = filter.AndAlso(x => x.Store.EnterpriseId == paginationRequestDto.EnterpriseId.Value && x.IsActive);

                if (paginationRequestDto.ResponsibleAuditorId.HasValue)
                    filter = filter.AndAlso(x => x.PeriodAuditParticipants.Any(a => a.UserReferenceId == paginationRequestDto.ResponsibleAuditorId.Value && a.RoleCodeSnapshot == RoleCodes.Auditor.Code) && x.IsActive);

                if (paginationRequestDto.StartDate.HasValue && paginationRequestDto.EndDate.HasValue)
                    filter = filter.AndAlso(x => x.CreationDate >= paginationRequestDto.StartDate.Value && x.CreationDate <= paginationRequestDto.EndDate.Value && x.IsActive);

                if (paginationRequestDto.AuditStatusId.HasValue)
                    filter = filter.AndAlso(x => x.StatusId == paginationRequestDto.AuditStatusId.Value && x.IsActive);

                if (paginationRequestDto.DocumentNumber != null)
                    filter = filter.AndAlso(x => x.CorrelativeNumber == paginationRequestDto.DocumentNumber);

                if (currentUser.RoleCodes.Contains(RoleCodes.StoreAdmin.Code))
                {
                    filter = filter.AndAlso(x => x.PeriodAuditParticipants.Any(p => p.UserReferenceId == currentUser.UserReferenceId && p.RoleCodeSnapshot == RoleCodes.StoreAdmin.Code && p.IsActive));
                }


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


                var userReference = await _userReferenceRepository.GetFirstOrDefaultAsync(filter: x => x.UserReferenceId == currentUser.UserReferenceId);
                foreach (var ent in pagedResult.Items)
                {
                    if (ent.Participants.Any(a => a.UserReferenceId == currentUser.UserReferenceId && a.RoleCodeSnapshot == RoleCodes.Auditor.Code))
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
                        includeProperties: [v => v.PeriodAuditParticipants, y => y.AuditStatus, z => z.Store]);
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
                        await BuildSendEmail.NotifySupervisorOfNewAudit(_emailService, periodAuditUpdate, _webAppSettings.Url);
                    }
                    if (periodAuditUpdate.StatusId == statusFinal?.AuditStatusId)
                    {
                        await BuildSendEmail.NotifyAllUserAudit(_emailService, periodAuditUpdate, _auditRoleConfigurationRepository, _periodAuditGroupResultRepository, _fileSettings, ent.Store?.Email, _webAppSettings.Url);
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

        public async Task<ResponseDto<PeriodAuditReportResponseDto>> Export(PeriodAuditExportRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditReportResponseDto>();
            try
            {
                var currentUser = _httpContextAccessor.CurrentUser();
                Expression<Func<PeriodAudit, bool>> filter = x => x.IsActive;

                // Parse dates from string
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!string.IsNullOrEmpty(requestDto.StartDate))
                {
                    if (DateTime.TryParse(requestDto.StartDate, out var parsedStart))
                        startDate = parsedStart;
                }

                if (!string.IsNullOrEmpty(requestDto.EndDate))
                {
                    if (DateTime.TryParse(requestDto.EndDate, out var parsedEnd))
                        endDate = parsedEnd;
                }


                if (!string.IsNullOrEmpty(requestDto.Filter))
                    filter = filter.AndAlso(x => x.GlobalObservations.Contains(requestDto.Filter) && x.IsActive);

                if (requestDto.StoreId.HasValue)
                    filter = filter.AndAlso(x => x.StoreId == requestDto.StoreId.Value && x.IsActive);
                if (requestDto.EnterpriseId.HasValue)
                    filter = filter.AndAlso(x => x.Store.EnterpriseId == requestDto.EnterpriseId.Value && x.IsActive);

                if (requestDto.ResponsibleAuditorId.HasValue)
                    filter = filter.AndAlso(x => x.PeriodAuditParticipants.Any(a => a.UserReferenceId == requestDto.ResponsibleAuditorId.Value && a.RoleCodeSnapshot == RoleCodes.Auditor.Code) && x.IsActive);

                if (startDate.HasValue && endDate.HasValue)
                    filter = filter.AndAlso(x => x.CreationDate >= startDate.Value && x.CreationDate <= endDate.Value && x.IsActive);

                if (requestDto.AuditStatusId.HasValue)
                    filter = filter.AndAlso(x => x.StatusId == requestDto.AuditStatusId.Value && x.IsActive);

                if (requestDto.DocumentNumber != null)
                    filter = filter.AndAlso(x => x.CorrelativeNumber == requestDto.DocumentNumber);

                if (currentUser.RoleCodes.Contains(RoleCodes.StoreAdmin.Code))
                {
                    filter = filter.AndAlso(x => x.PeriodAuditParticipants.Any(p => p.UserReferenceId == currentUser.UserReferenceId && p.RoleCodeSnapshot == RoleCodes.StoreAdmin.Code && p.IsActive));
                }

                if (requestDto.PeriodAuditIds != null && requestDto.PeriodAuditIds.Length != 0)
                {
                    filter = filter.AndAlso(x => requestDto.PeriodAuditIds.Contains(x.PeriodAuditId) && x.IsActive);
                }

                Func<IQueryable<PeriodAudit>, IOrderedQueryable<PeriodAudit>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var exportData = await _periodAuditRepository.GetWithScaleGroup(filter: filter);

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Auditorías");

                // Configurar encabezados
                var headers = new[]
                {
                    "Nº auditoría", "Empresa", "Tienda", "Jefe de área", "Auditor responsable", "Fecha de registro", "Fecha de auditoría", "Días auditados", "Estado", "Calificación", "Calificación %",
                    "Grupo", "Nivel de riesgo", "Calificación", "Peso/Ponderación", "Código", "Punto auditable", "Nivel de riesgo", "Calificación", "Peso/Ponderación"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                }

                // Llenar datos
                int row = 2;
                foreach (var item in exportData)
                {
                    // Detalles de grupos y puntos auditables
                    foreach (var groupResult in item.PeriodAuditGroupResults)
                    {
                        bool isFirstPoint = true;
                        foreach (var scaResult in groupResult.PeriodAuditScaleResults)
                        {
                            if (!isFirstPoint)
                            {
                                row++;
                            }
                            worksheet.Cell(row, 1).Value = item.CorrelativeNumber ?? "";
                            worksheet.Cell(row, 2).Value = item.Store?.Enterprise?.Name ?? "";
                            worksheet.Cell(row, 3).Value = item.Store?.Name;
                            worksheet.Cell(row, 4).Value = item.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JefeDeArea.Code)?.UserReference?.FullName ?? "";
                            worksheet.Cell(row, 5).Value = item.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.Auditor.Code)?.UserReference?.FullName ?? "";
                            worksheet.Cell(row, 6).Value = item.CreationDate.ToString("dd/MM/yyyy");
                            worksheet.Cell(row, 7).Value = item.ReportDate?.ToString("dd/MM/yyyy") ?? "";
                            worksheet.Cell(row, 8).Value = item.AuditedDays;
                            worksheet.Cell(row, 9).Value = item.AuditStatus?.Name ?? "";
                            worksheet.Cell(row, 10).Value = item.ScaleName ?? "";
                            worksheet.Cell(row, 11).Value = item.ScoreValue;
                            worksheet.Cell(row, 12).Value = groupResult?.Group?.Name ?? "";
                            worksheet.Cell(row, 13).Value = groupResult?.ScaleDescription ?? "";
                            worksheet.Cell(row, 14).Value = groupResult?.ScoreValue ?? 0;
                            worksheet.Cell(row, 15).Value = groupResult?.TotalWeighting ?? 0;
                            worksheet.Cell(row, 16).Value = scaResult.ScaleGroup?.Code ?? "";
                            worksheet.Cell(row, 17).Value = scaResult.ScaleGroup?.Name ?? "";
                            worksheet.Cell(row, 18).Value = scaResult.ScaleDescription ?? "";
                            worksheet.Cell(row, 19).Value = scaResult.ScoreValue;
                            worksheet.Cell(row, 20).Value = scaResult.AppliedWeighting;

                            // Aplicar color al estado si existe
                            if (!string.IsNullOrEmpty(item.AuditStatus?.ColorCode))
                            {
                                try
                                {
                                    var statusColor = System.Drawing.ColorTranslator.FromHtml(item.AuditStatus?.ColorCode ?? "#FFFFFF");
                                    worksheet.Cell(row, 9).Style.Fill.BackgroundColor = XLColor.FromColor(statusColor);

                                    // Texto blanco si el color de fondo es oscuro
                                    var luminance = (0.299 * statusColor.R + 0.587 * statusColor.G + 0.114 * statusColor.B) / 255;
                                    if (luminance < 0.5)
                                    {
                                        worksheet.Cell(row, 9).Style.Font.FontColor = XLColor.White;
                                    }
                                }
                                catch
                                {
                                    // Ignorar si el color no es válido
                                }
                            }

                            isFirstPoint = false;
                        }
                    }

                    row++;
                }

                // Aplicar formato
                var dataRange = worksheet.Range(1, 1, row - 1, headers.Length);
                dataRange.Style.Font.FontName = "Arial";
                dataRange.Style.Font.FontSize = 10;
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Formato de encabezados
                var headerRange = worksheet.Range(1, 1, 1, headers.Length);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(84, 130, 53);
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Ajustar ancho de columnas
                worksheet.Column(1).Width = 18;  // Nro. Documento
                worksheet.Column(2).Width = 30;  // Empresa
                worksheet.Column(3).Width = 25;  // Tienda
                worksheet.Column(4).Width = 25;  // Jefe de área
                worksheet.Column(5).Width = 25;  // Auditor responsable
                worksheet.Column(6).Width = 18;  // Fecha de registro
                worksheet.Column(7).Width = 18;  // Fecha de auditoría
                worksheet.Column(8).Width = 15;  // Días auditados
                worksheet.Column(9).Width = 20;  // Estado
                worksheet.Column(10).Width = 15; // Calificación
                worksheet.Column(11).Width = 15; // Calificación %
                worksheet.Column(12).Width = 15; // Grupo
                worksheet.Column(13).Width = 25; // Nivel de riesgo
                worksheet.Column(14).Width = 15; // Calificación
                worksheet.Column(15).Width = 15; // Peso/Ponderación
                worksheet.Column(16).Width = 25; // Código
                worksheet.Column(17).Width = 25; // Punto auditable
                worksheet.Column(18).Width = 15; // Nivel de riesgo
                worksheet.Column(19).Width = 15; // Calificación
                worksheet.Column(20).Width = 15; // Peso/Ponderación

                // Congelar fila de encabezados
                worksheet.SheetView.FreezeRows(1);

                // Filtros automáticos
                worksheet.RangeUsed().SetAutoFilter();

                // Convertir a Base64
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var fileBytes = stream.ToArray();
                var fileBase64 = Convert.ToBase64String(fileBytes);

                var resultExport = new PeriodAuditReportResponseDto
                {
                    FileBase64 = fileBase64,
                    FileName = $"Auditorias-{DateTime.Now:yyyyMMddHHmmss}.xlsx",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };

                response.Data = resultExport;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error exportando auditorías: {ex.Message}");
                response = ResponseDto.Error<PeriodAuditReportResponseDto>(ex.Message);
            }
            return response;
        }
    }
}
