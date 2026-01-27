using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.PeriodAudit;
using Rokys.Audit.DTOs.Responses.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Responses.ScaleGroup;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class PeriodAuditGroupResultService : IPeriodAuditGroupResultService
    {
        private readonly IPeriodAuditGroupResultRepository _periodAuditGroupResultRepository;
        private readonly IValidator<PeriodAuditGroupResultRequestDto> _validator;
        private readonly ILogger<PeriodAuditGroupResultService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IScaleGroupRepository _scaleGroupRepository;
        private readonly IPeriodAuditScaleResultRepository _periodAuditScaleResultRepository;
        private readonly ITableScaleTemplateRepository _tableScaleTemplateRepository;
        private readonly IPeriodAuditTableScaleTemplateResultRepository _periodAuditTableScaleTemplateResultRepository;
        private readonly IAuditTemplateFieldRepository _auditTemplateFieldRepository;
        private readonly IPeriodAuditFieldValuesRepository _periodAuditFieldValuesRepository;
        private readonly ICriteriaSubResultRepository _criteriaSubResultRepository;
        private readonly IPeriodAuditScaleSubResultRepository _periodAuditScaleSubResultRepository;
        private readonly IPeriodAuditScoringCriteriaResultRepository _periodAuditScoringCriteriaResultRepository;
        private readonly IScoringCriteriaRepository _scoringCriteriaRepository;
        private readonly IScaleCompanyRepository _scaleCompanyRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IPeriodAuditRepository _periodAuditRepository;
        private readonly IPeriodAuditActionPlanRepository _periodAuditActionPlanRepository;
        private readonly IAuditStatusRepository _auditStatusRepository;
        private readonly IUserReferenceRepository _userReferenceRepository;
        private readonly IEnterpriseGroupRepository _enterpriseGroupRepository;
        private readonly ISystemConfigurationRepository _systemConfigurationRepository;

        private readonly IPeriodAuditActionPlanService _periodAuditActionPlanService;
        public PeriodAuditGroupResultService(
            IPeriodAuditGroupResultRepository repository,
            IValidator<PeriodAuditGroupResultRequestDto> validator,
            ILogger<PeriodAuditGroupResultService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IScaleGroupRepository scaleGroupRepository,
            IPeriodAuditScaleResultRepository periodAuditScaleResultRepository,
            ITableScaleTemplateRepository tableScaleTemplateRepository,
            IPeriodAuditTableScaleTemplateResultRepository periodAuditTableScaleTemplateResultRepository,
            IAuditTemplateFieldRepository auditTemplateFieldRepository,
            IPeriodAuditFieldValuesRepository periodAuditFieldValuesRepository,
            ICriteriaSubResultRepository criteriaSubResultRepository,
            IPeriodAuditScaleSubResultRepository periodAuditScaleSubResultRepository,
            IPeriodAuditScoringCriteriaResultRepository periodAuditScoringCriteriaResultRepository,
            IScoringCriteriaRepository scoringCriteriaRepository,
            IScaleCompanyRepository scaleCompanyRepository,
            IServiceScopeFactory serviceScopeFactory,
            IPeriodAuditRepository periodAuditRepository,
            IPeriodAuditActionPlanRepository periodAuditActionPlanRepository,
            IAuditStatusRepository auditStatusRepository,
            IUserReferenceRepository userReferenceRepository,
            IEnterpriseGroupRepository enterpriseGroupRepository,
            ISystemConfigurationRepository systemConfigurationRepository,
            IPeriodAuditActionPlanService periodAuditActionPlanService
        )
        {
            _periodAuditGroupResultRepository = repository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _scaleGroupRepository = scaleGroupRepository;
            _periodAuditScaleResultRepository = periodAuditScaleResultRepository;
            _tableScaleTemplateRepository = tableScaleTemplateRepository;
            _periodAuditTableScaleTemplateResultRepository = periodAuditTableScaleTemplateResultRepository;
            _auditTemplateFieldRepository = auditTemplateFieldRepository;
            _periodAuditFieldValuesRepository = periodAuditFieldValuesRepository;
            _criteriaSubResultRepository = criteriaSubResultRepository;
            _periodAuditScaleSubResultRepository = periodAuditScaleSubResultRepository;
            _periodAuditScoringCriteriaResultRepository = periodAuditScoringCriteriaResultRepository;
            _scoringCriteriaRepository = scoringCriteriaRepository;
            _scaleCompanyRepository = scaleCompanyRepository;
            _serviceScopeFactory = serviceScopeFactory;
            _periodAuditRepository = periodAuditRepository;
            _periodAuditActionPlanRepository = periodAuditActionPlanRepository;
            _auditStatusRepository = auditStatusRepository;
            _userReferenceRepository = userReferenceRepository;
            _enterpriseGroupRepository = enterpriseGroupRepository;
            _systemConfigurationRepository = systemConfigurationRepository;
            _periodAuditActionPlanService = periodAuditActionPlanService;
        }
        public async Task<ResponseDto<PeriodAuditGroupResultResponseDto>> Create(PeriodAuditGroupResultRequestDto requestDto, bool isTrasacction = false)
        {
            var response = ResponseDto.Create<PeriodAuditGroupResultResponseDto>();
            try
            {
                var validate = await _validator.ValidateAsync(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                requestDto.ScoreValue = 0;

                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<PeriodAuditGroupResult>(requestDto);

                // Assign SortOrder server-side grouped by PeriodAuditId
                var existingSortOrders = (await _periodAuditGroupResultRepository.GetAsync(filter: x => x.PeriodAuditId == requestDto.PeriodAuditId && x.IsActive))
                    .Select(x => x.SortOrder);
                entity.SortOrder = Rokys.Audit.Common.Helpers.SortOrderHelper.GetNextSortOrder(existingSortOrders);

                entity.CreateAudit(currentUser.UserName);
                entity.IsActive = true;
                _periodAuditGroupResultRepository.Insert(entity);

                var scaleGroups = await _scaleGroupRepository.GetByGroupIdAsync(requestDto.GroupId);
                var currentPeriodAuditGroupResult = await _periodAuditGroupResultRepository.GetByPeriodAuditIdAsync(requestDto.PeriodAuditId);
                var currentWeighting = currentPeriodAuditGroupResult.Sum(x => x.TotalWeighting);
                if(currentWeighting + requestDto.TotalWeighting > 100)
                {
                    response = ResponseDto.Error<PeriodAuditGroupResultResponseDto>($"Ya tiene asignado {currentWeighting}% de ponderación, no se puede asignar una ponderación total de {requestDto.TotalWeighting + currentWeighting}%.");
                    return response;
                }
                if (scaleGroups == null)
                {
                    response = ResponseDto.Error<PeriodAuditGroupResultResponseDto>("No se encontró el grupo de escala asociado al grupo.");
                    return response;
                }
                if (requestDto.StartDate == null && requestDto.EndDate == null)
                {
                    var periodAudit = await _periodAuditRepository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == requestDto.PeriodAuditId && x.IsActive);
                    if (periodAudit != null)
                    {
                        requestDto.StartDate = periodAudit.StartDate;
                        requestDto.EndDate = periodAudit.EndDate;
                    }
                }

                foreach (var scale in scaleGroups)
                {
                    var periodAuditScaleResult = new PeriodAuditScaleResult
                    {
                        PeriodAuditGroupResultId = entity.PeriodAuditGroupResultId,
                        ScaleGroupId = scale.ScaleGroupId,
                        AppliedWeighting = scale.Weighting,
                        ScoreValue = 0,
                        SortOrder = scale.SortOrder,
                        Recommendation = scale.Recommendation,
                        Impact = scale.Impact
                    };
                    periodAuditScaleResult.CreateAudit(currentUser.UserName);
                    _periodAuditScaleResultRepository.Insert(periodAuditScaleResult);

                    var scaleResponse = _mapper.Map<ScaleGroupResponseDto>(scale);
                    var periodAuditScaleResultResponse = _mapper.Map<PeriodAuditScaleResultResponseDto>(periodAuditScaleResult);
                    await CreateTableScaleTemplateResults(scaleResponse, periodAuditScaleResultResponse, requestDto.StartDate, requestDto.EndDate);
                }
                if (!isTrasacction)
                    await _unitOfWork.CommitAsync();

                var createdEntity = await _periodAuditGroupResultRepository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditGroupResultId == entity.PeriodAuditGroupResultId && x.IsActive,
                    includeProperties: [x => x.Group, y => y.PeriodAudit]);
                response.Data = _mapper.Map<PeriodAuditGroupResultResponseDto>(createdEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditGroupResultResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _periodAuditGroupResultRepository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditGroupResultId == id && x.IsActive);

                if (entity == null)
                    return ResponseDto.Error("No se encontró el registro.");

                if (entity.ScoreValue > 0)
                    return ResponseDto.Error("No se puede eliminar un resultado de grupo de auditoría que ya tiene una puntuación asignada.");

                var scaleResults = await _periodAuditScaleResultRepository.GetAsync(x => x.PeriodAuditGroupResultId == entity.PeriodAuditGroupResultId && x.IsActive);
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
                            _periodAuditFieldValuesRepository.Delete(fieldValue);
                        }

                        tableResult.IsActive = false;
                        tableResult.UpdateDate = DateTime.Now;
                        _periodAuditTableScaleTemplateResultRepository.Delete(tableResult);
                    }

                    var subResults = await _periodAuditScaleSubResultRepository.GetAsync(x => x.PeriodAuditScaleResultId == scaleResult.PeriodAuditScaleResultId && x.IsActive);
                    foreach (var subResult in subResults)
                    {
                        subResult.IsActive = false;
                        subResult.UpdateDate = DateTime.Now;
                        _periodAuditScaleSubResultRepository.Delete(subResult);
                    }

                    var scoringResults = await _periodAuditScoringCriteriaResultRepository.GetAsync(x => x.PeriodAuditScaleResultId == scaleResult.PeriodAuditScaleResultId && x.IsActive);
                    foreach (var scoringResult in scoringResults)
                    {
                        scoringResult.IsActive = false;
                        scoringResult.UpdateDate = DateTime.Now;
                        _periodAuditScoringCriteriaResultRepository.Delete(scoringResult);
                    }

                    scaleResult.IsActive = false;
                    scaleResult.UpdateDate = DateTime.Now;
                    _periodAuditScaleResultRepository.Update(scaleResult);
                }

                // Finalmente desactivar el GroupResult principal
                entity.IsActive = false;
                entity.UpdateDate = DateTime.Now;
                _periodAuditGroupResultRepository.Update(entity);

                await _unitOfWork.CommitAsync();
                response.WithMessage("El grupo de resultados y sus dependencias fueron eliminados correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }

            return response;
        }

        public async Task<ResponseDto<PeriodAuditGroupResultResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<PeriodAuditGroupResultResponseDto>();
            try
            {
                var entity = await _periodAuditGroupResultRepository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditGroupResultId == id && x.IsActive, 
                    includeProperties: [ x => x.Group, y => y.PeriodAudit]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditGroupResultResponseDto>("No se encontró el registro.");
                    return response;
                }
                response.Data = _mapper.Map<PeriodAuditGroupResultResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditGroupResultResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditGroupResultResponseDto>> Update(Guid id, UpdatePeriodAuditGroupResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditGroupResultResponseDto>();
            try
            {
                var entity = await _periodAuditGroupResultRepository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditGroupResultId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditGroupResultResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentPeriodAuditGroupResult = await _periodAuditGroupResultRepository.GetByPeriodAuditIdAsync(entity.PeriodAuditId, id);
                var currentWeighting = currentPeriodAuditGroupResult.Sum(x => x.PeriodAuditGroupResultId == id ? 0 : x.TotalWeighting);
                if (currentWeighting + requestDto.TotalWeighting > 100)
                {
                    response = ResponseDto.Error<PeriodAuditGroupResultResponseDto>($"Ya tiene asignado {currentWeighting}% de ponderación, no se puede asignar una ponderación total de {requestDto.TotalWeighting + currentWeighting}%.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity.TotalWeighting = requestDto.TotalWeighting;
                entity.UpdateAudit(currentUser.UserName);
                _periodAuditGroupResultRepository.Update(entity);
                
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<PeriodAuditGroupResultResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditGroupResultResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<PeriodAuditGroupResultResponseDto>>> GetPaged(PeriodAuditGroupResultFilterRequestDto filterRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<PeriodAuditGroupResultResponseDto>>();
            try
            {
                Expression<Func<PeriodAuditGroupResult, bool>> filter = x => x.IsActive;
                if (filterRequestDto.PeriodAuditId.HasValue)
                {
                    filter = filter.AndAlso(x => x.PeriodAuditId == filterRequestDto.PeriodAuditId.Value);
                }
                if (filterRequestDto.GroupId.HasValue)
                {
                    filter = filter.AndAlso(x => x.GroupId == filterRequestDto.GroupId.Value);
                }
                if (!string.IsNullOrEmpty(filterRequestDto.Filter))
                {
                    filter = filter.AndAlso(x => (x.ScaleDescription.Contains(filterRequestDto.Filter) || x.Observations.Contains(filterRequestDto.Filter)));
                }

                Func<IQueryable<PeriodAuditGroupResult>, IOrderedQueryable<PeriodAuditGroupResult>> orderBy = q => q.OrderBy(x => x.SortOrder);
                var entities = await _periodAuditGroupResultRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: filterRequestDto.PageNumber,
                    pageSize: filterRequestDto.PageSize,
                    includeProperties: [ x => x.Group, x => x.PeriodAudit]

                );
                var pagedResult = new PaginationResponseDto<PeriodAuditGroupResultResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<PeriodAuditGroupResultResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = filterRequestDto.PageNumber,
                    PageSize = filterRequestDto.PageSize
                };
                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<PeriodAuditGroupResultResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<bool>> Recalculate(Guid periodAuditGroupResultId)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                var entity = await _periodAuditGroupResultRepository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditGroupResultId == periodAuditGroupResultId && x.IsActive,
                    includeProperties: [x => x.Group, y => y.PeriodAudit.Store]);

                var periodAuditScaleResult = await _periodAuditScaleResultRepository.GetByPeriodAuditGroupResultId(periodAuditGroupResultId);
                decimal acumulatedScore = 0;
                foreach (var scaleResult in periodAuditScaleResult)
                {
                    var scaleScore = (scaleResult.AppliedWeighting / 100) *  scaleResult.ScoreValue;
                    acumulatedScore += scaleScore;
                }

                var scaleCompany = await _scaleCompanyRepository.GetConfiguredForEnterprise(entity.PeriodAudit.Store!.Enterprise!.EnterpriseGroups!.FirstOrDefault(e => e.IsActive)!.EnterpriseGroupingId, entity.PeriodAudit.Store.EnterpriseId);
                if (scaleCompany == null || !scaleCompany.Any())
                {
                    response = ResponseDto.Error<bool>("No se encontró la escala asociada a la empresa ni la escala por defecto.");
                    return response;
                }

                bool scaleFound = false;
                foreach (var scale in scaleCompany)
                {
                    if (acumulatedScore >= scale.MinValue && acumulatedScore <= scale.MaxValue)
                    {
                        entity.ScaleDescription = scale.Name;
                        entity.ScaleColor = scale.ColorCode;
                        scaleFound = true;
                        break;
                    }
                }
                if (!scaleFound)
                {
                    response = ResponseDto.Error<bool>("No se encontró una escala que coincida con el puntaje obtenido.");
                    return response;
                }

                entity.ScoreValue = acumulatedScore;
                _periodAuditGroupResultRepository.Update(entity);

                await _unitOfWork.CommitAsync();

                await using (var scope = _serviceScopeFactory.CreateAsyncScope())
                {
                    var auditService = scope.ServiceProvider.GetRequiredService<IPeriodAuditService>();
                    var responseAudit = await auditService.Recalculate(entity.PeriodAuditId);

                    if (responseAudit.IsValid)
                        response.Data = true;
                    else
                        response.Messages.AddRange(responseAudit.Messages);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<bool>(ex.Message);
            }
            return response;
        }

        public async Task CreateTableScaleTemplateResults(ScaleGroupResponseDto scale, PeriodAuditScaleResultResponseDto periodAuditScaleResult, DateTime? startDate = null, DateTime? endDate = null)
        {
            var currentUser = _httpContextAccessor.CurrentUser();
            var tableScaleTemplates = await _tableScaleTemplateRepository.GetByScaleGroupId(periodAuditScaleResult.ScaleGroupId);
            if (tableScaleTemplates != null && tableScaleTemplates.Any())
            {
                foreach (var template in tableScaleTemplates)
                {
                    var periodAuditTableScaleTemplateResult = new PeriodAuditTableScaleTemplateResult
                    {
                        PeriodAuditScaleResultId = periodAuditScaleResult.PeriodAuditScaleResultId,
                        TableScaleTemplateId = template.TableScaleTemplateId,
                        Code = template.Code,
                        Name = template.Name,
                        Orientation = template.Orientation,
                        TemplateData = template.TemplateData,
                        SortOrder = template.SortOrder
                    };
                    periodAuditTableScaleTemplateResult.CreateAudit(currentUser.UserName);
                    _periodAuditTableScaleTemplateResultRepository.Insert(periodAuditTableScaleTemplateResult);

                    var auditTemplateField = await _auditTemplateFieldRepository.GetByTemplateId(template.TableScaleTemplateId);
                    if (auditTemplateField != null && auditTemplateField.Any())
                    {
                        foreach (var field in auditTemplateField)
                        {
                            var defaultValue = field.DefaultValue;

                            if (field.FieldType == FieldConstants.Date && field.FieldCode == FieldConstants.From && startDate != null && field.DefaultValue != null)
                            {
                                defaultValue = startDate?.ToString("yyyy-MM-dd");
                            }
                            if (field.FieldType == FieldConstants.Date && field.FieldCode == FieldConstants.To && endDate != null && field.DefaultValue != null)
                            {
                                defaultValue = endDate?.ToString("yyyy-MM-dd");
                            }
                            if (field.FieldType == FieldConstants.Numeric && field.FieldCode == FieldConstants.DaysAudit && startDate.HasValue && endDate.HasValue)
                            {
                                defaultValue = ((endDate.Value - startDate.Value).Days + 1).ToString();
                            }

                            var periodAuditFieldValue = new PeriodAuditFieldValues
                            {
                                PeriodAuditTableScaleTemplateResultId = periodAuditTableScaleTemplateResult.PeriodAuditTableScaleTemplateResultId,
                                AuditTemplateFieldId = field.AuditTemplateFieldId,
                                FieldCode = field.FieldCode,
                                FieldName = field.FieldName,
                                FieldType = field.FieldType,
                                IsCalculated = field.IsCalculated,
                                CalculationFormula = field.CalculationFormula,
                                AcumulationType = field.AcumulationType,
                                FieldOptions = field.FieldOptions,
                                SortOrder = field.SortOrder,
                                DefaultValue = defaultValue
                            };
                            periodAuditFieldValue.CreateAudit(currentUser.UserName);
                            _periodAuditFieldValuesRepository.Insert(periodAuditFieldValue);
                        }
                    }
                }
            }
            var criteriaSubResults = await _criteriaSubResultRepository.GetByScaleGroupIdAsync(scale.ScaleGroupId);
            if (criteriaSubResults != null && criteriaSubResults.Any())
            {
                foreach (var criteriaSubResult in criteriaSubResults)
                {
                    var periodAuditScaleSubResult = new PeriodAuditScaleSubResult
                    {
                        PeriodAuditScaleResultId = periodAuditScaleResult.PeriodAuditScaleResultId,
                        CriteriaSubResultId = criteriaSubResult.CriteriaSubResultId,
                        CriteriaName = criteriaSubResult.CriteriaName,
                        CriteriaCode = criteriaSubResult.CriteriaCode,
                        ColorCode = criteriaSubResult.ColorCode,
                        ForSummary = criteriaSubResult.ForSummary,
                        AppliedFormula = criteriaSubResult.ResultFormula,
                        ScoreObtained = 0,
                    };
                    periodAuditScaleSubResult.CreateAudit(currentUser.UserName);
                    _periodAuditScaleSubResultRepository.Insert(periodAuditScaleSubResult);
                }
            }

            var scoringCriterias = await _scoringCriteriaRepository.GetByScaleGroupIdAsync(scale.ScaleGroupId);
            if (scoringCriterias != null && scoringCriterias.Any())
            {
                foreach (var scoringCriteria in scoringCriterias)
                {
                    var periodAuditScoringCriteriaResult = new PeriodAuditScoringCriteriaResult
                    {
                        PeriodAuditScaleResultId = periodAuditScaleResult.PeriodAuditScaleResultId,
                        CriteriaName = scoringCriteria.CriteriaName,
                        CriteriaCode = scoringCriteria.CriteriaCode,
                        ResultFormula = scoringCriteria.ResultFormula,
                        ComparisonOperator = scoringCriteria.ComparisonOperator,
                        ExpectedValue = scoringCriteria.ExpectedValue,
                        Score = scoringCriteria.Score,
                        SortOrder = scoringCriteria.SortOrder,
                        ResultObtained = null,
                    };
                    periodAuditScoringCriteriaResult.CreateAudit(currentUser.UserName);
                    _periodAuditScoringCriteriaResultRepository.Insert(periodAuditScoringCriteriaResult);
                }
            }
        }

        public async Task<ResponseDto<bool>> ChangeOrder(Guid periodAuditId, int currentPosition, int newPosition)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                var items = (await _periodAuditGroupResultRepository.GetAsync(filter: x => x.PeriodAuditId == periodAuditId && x.IsActive))
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                var currentIndex = items.FindIndex(x => x.SortOrder == currentPosition);
                var newIndex = items.FindIndex(x => x.SortOrder == newPosition);
                if (currentIndex < 0 || newIndex < 0)
                {
                    response = ResponseDto.Error<bool>("SortOrder no encontrado en el resultado de auditoría del período.");
                    return response;
                }

                var item = items[currentIndex];
                items.RemoveAt(currentIndex);
                items.Insert(newIndex, item);

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SortOrder = i + 1;
                    _periodAuditGroupResultRepository.Update(items[i]);
                }
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

        public async Task<ResponseDto<int>> SyncActionPlans(Guid periodAuditGroupResultId, PeriodAuditUpdateActionPlanRequestDto requestDto)
        {
            var response = ResponseDto.Create<int>();
            try
            {
                if (requestDto.PeriodAuditActionPlans == null || !requestDto.PeriodAuditActionPlans.Any())
                {
                    response = ResponseDto.Error<int>("No se proporcionaron planes de acción para sincronizar.");
                    return response;
                }

                var entity = await _periodAuditGroupResultRepository.GetFirstOrDefaultAsync(x => x.PeriodAuditGroupResultId == periodAuditGroupResultId && x.IsActive, includeProperties: [x => x.PeriodAudit.AuditStatus, x => x.PeriodAudit.PeriodAuditParticipants, x => x.PeriodAudit.Store]);
                if (entity == null)
                {
                    response = ResponseDto.Error<int>("No se encontró el registro.");
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                var currentUserName = currentUser.UserName;

                if (entity.PeriodAudit?.ActionPlanCompletedDate != null && !currentUser.RoleCodes.Contains(RoleCodes.JefeDeArea.Code))
                {
                    response = ResponseDto.Error<int>("No se pueden gestionar planes de acción en una auditoría que ya ha completado la gestión de planes de acción.");
                    return response;
                }

                var resp = await GetAllowedActionPlans(entity.PeriodAuditId);

                if (!resp.IsValid || !resp.Data)
                {
                    response = ResponseDto.Error<int>(resp.Messages.FirstOrDefault()?.Message ?? "No se pudo validar si el usuario tiene permisos para gestionar planes de acción.");
                    return response;
                }

                // validar que existe una configuracion de puntaje para aplicar planes de acción
                var enterpriseConfigResponse = await _periodAuditActionPlanService.GetEnterpriseConfigurationByPeriodAuditId(entity.PeriodAuditId);
                if (!enterpriseConfigResponse.IsValid || enterpriseConfigResponse.Data == null || !enterpriseConfigResponse.Data.HasConfiguration)
                {
                    response = ResponseDto.Error<int>("No se encontró la configuración de puntaje para aplicar planes de acción en la empresa asociada a la auditoría.");
                    return response;
                }

                var configValue = enterpriseConfigResponse.Data.ConfigurationValue;

                // Validar que todos los usuarios responsables existen
                var responsibleUserIds = requestDto.PeriodAuditActionPlans
                    .Select(x => x.ResponsibleUserId)
                    .Distinct()
                    .ToList();

                foreach (var userId in responsibleUserIds)
                {
                    var userExists = await _userReferenceRepository.GetFirstOrDefaultAsync(filter: x => x.UserReferenceId == userId);
                    if (userExists == null)
                    {
                        response = ResponseDto.Error<int>($"El usuario responsable con ID {userId} no existe en el sistema.");
                        return response;
                    }
                }

                int actionPlansCount = 0;
                foreach (var actionPlanDto in requestDto.PeriodAuditActionPlans)
                {
                    // Validar que el ScoreValue del PeriodAuditScaleResult sea menor al valor configurado
                    var scaleResult = await _periodAuditScaleResultRepository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditScaleResultId == actionPlanDto.PeriodAuditScaleResultId && x.IsActive);
                    if (scaleResult == null)
                    {
                        response = ResponseDto.Error<int>($"No se encontró el resultado de escala con ID {actionPlanDto.PeriodAuditScaleResultId}.");
                        return response;
                    }

                    if (scaleResult.ScoreValue >= configValue)
                    {
                        response = ResponseDto.Error<int>($"El puntaje obtenido ({scaleResult.ScoreValue}) debe ser menor a {configValue} para poder crear o editar planes de acción en este resultado de escala.");
                        return response;
                    }

                    var existingActionPlan = await _periodAuditActionPlanRepository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditScaleResultId == actionPlanDto.PeriodAuditScaleResultId);

                    if (existingActionPlan != null)
                    {
                        // Update existing action plan
                        existingActionPlan.DisiplinaryMeasureTypeId = actionPlanDto.DisiplinaryMeasureTypeId;
                        existingActionPlan.ResponsibleUserId = actionPlanDto.ResponsibleUserId;
                        existingActionPlan.Description = actionPlanDto.Description;
                        existingActionPlan.DueDate = actionPlanDto.DueDate;
                        existingActionPlan.ApplyMeasure = actionPlanDto.ApplyMeasure;

                        existingActionPlan.UpdateAudit(currentUserName);

                        _periodAuditActionPlanRepository.Update(existingActionPlan);
                    }
                    else
                    {
                        // Create new action plan
                        var newActionPlan = new PeriodAuditActionPlan
                        {
                            PeriodAuditScaleResultId = actionPlanDto.PeriodAuditScaleResultId,
                            DisiplinaryMeasureTypeId = actionPlanDto.DisiplinaryMeasureTypeId,
                            ResponsibleUserId = actionPlanDto.ResponsibleUserId,
                            Description = actionPlanDto.Description,
                            DueDate = actionPlanDto.DueDate,
                            ApplyMeasure = actionPlanDto.ApplyMeasure
                        };

                        newActionPlan.CreateAudit(currentUserName);

                        _periodAuditActionPlanRepository.Insert(newActionPlan);
                    }
                    actionPlansCount++;
                }

                await _unitOfWork.CommitAsync();
                response.Data = actionPlansCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<int>(ex.Message);
            }
            return response;
        }
    
        public async Task<ResponseDto<bool>> GetAllowedActionPlans(Guid id)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                var entity = await _periodAuditRepository.GetCustomByIdAsync(filter: x => x.PeriodAuditId == id && x.IsActive);
                
                if (entity == null)
                {
                    throw new Exception("No se encontró el registro.");
                }

                var currentUser = _httpContextAccessor.CurrentUser();

                // Definir roles permitidos para crear y editar planes de acción
                var allowedEditRoles = new List<string> { RoleCodes.JobSupervisor.Code, RoleCodes.AssistantAdministrative.Code, RoleCodes.StoreAdmin.Code, RoleCodes.Auditor.Code };

                if (currentUser == null)
                {
                    throw new Exception("No se encontró la información del usuario actual.");
                }

                // Validar si el usuario es Jefe de Área
                if (currentUser.RoleCodes.Contains(RoleCodes.JefeDeArea.Code))
                {
                    response.Data = true;
                    return response;
                }

                // Validar que el usuario sea participante de la auditoría con uno de los roles autorizados
                if (entity.PeriodAuditParticipants == null || !entity.PeriodAuditParticipants.Any())
                {
                   throw new Exception("La auditoría no tiene participantes asignados.");
                }

                var userParticipant = entity.PeriodAuditParticipants
                    .FirstOrDefault(p => p.UserReferenceId == currentUser.UserReferenceId && p.IsActive);

                if (userParticipant == null)
                {
                    throw new Exception("El usuario actual no es participante de la auditoría.");
                }

                var userRoleInAudit = userParticipant.RoleCodeSnapshot;
                if (string.IsNullOrEmpty(userRoleInAudit) || !allowedEditRoles.Contains(userRoleInAudit))
                {
                    throw new Exception("El usuario no tiene un rol autorizado para realizar esta acción. Se requiere uno de los siguientes roles: Supervisor (A006), Asistente Administrativo (A004), Administrador de tienda (A002) o Auditor (A005).");
                }

                response.Data = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<bool>(ex.Message);
            }
            return response;
        }
    }
}
