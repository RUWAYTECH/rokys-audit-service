using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;
using Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Responses.Common;
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
    public class PeriodAuditScaleResultService : IPeriodAuditScaleResultService
    {
        private readonly IPeriodAuditScaleResultRepository _repository;
        private readonly IValidator<PeriodAuditScaleResultRequestDto> _validator;
        private readonly ILogger<PeriodAuditScaleResultService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPeriodAuditTableScaleTemplateResultRepository _periodAuditTableScaleTemplateResultRepository;
        private readonly IPeriodAuditFieldValuesRepository _fieldValuesRepository;
        private readonly IScoringCriteriaRepository _scoringCriteriaRepository;
        private readonly IScaleCompanyRepository _scaleCompanyRepository;
        private readonly IPeriodAuditScoringCriteriaResultRepository _periodAuditScoringCriteriaResultRepository;
        private readonly IPeriodAuditScaleSubResultRepository _periodAuditScaleSubResultRepository;
        private readonly IPeriodAuditGroupResultService _periodAuditGroupResultService;
        private readonly IStorageFilesRepository _storageFilesRepository;
        private readonly IStorageFilesService _storageFilesService;
        private readonly IUserReferenceRepository _userReferenceRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITableScaleTemplateRepository _tableScaleTemplateRepository;
        private readonly IAuditTemplateFieldRepository _auditTemplateFieldRepository;
        private readonly IPeriodAuditFieldValuesRepository _periodAuditFieldValuesRepository;
        private readonly ICriteriaSubResultRepository _criteriaSubResultRepository;
        private readonly IPeriodAuditRepository _periodAuditRepository;
        private readonly IScaleGroupRepository _scaleGroupRepository;

        public PeriodAuditScaleResultService(
            IPeriodAuditScaleResultRepository repository,
            IValidator<PeriodAuditScaleResultRequestDto> validator,
            ILogger<PeriodAuditScaleResultService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IPeriodAuditTableScaleTemplateResultRepository periodAuditTableScaleTemplateResultRepository,
            IPeriodAuditFieldValuesRepository fieldValuesRepository,
            IScoringCriteriaRepository scoringCriteriaRepository,
            IScaleCompanyRepository scaleCompanyRepository,
            IPeriodAuditScoringCriteriaResultRepository periodAuditScoringCriteriaResultRepository,
            IPeriodAuditScaleSubResultRepository periodAuditScaleSubResultRepository,
            IPeriodAuditGroupResultService periodAuditGroupResultService,
            IStorageFilesRepository storageFilesRepository,
            IStorageFilesService storageFilesService,
            IUserReferenceRepository userReferenceRepository,
            IServiceScopeFactory serviceScopeFactory,
            ITableScaleTemplateRepository tableScaleTemplateRepository,
            IAuditTemplateFieldRepository auditTemplateFieldRepository,
            IPeriodAuditFieldValuesRepository periodAuditFieldValuesRepository,
            ICriteriaSubResultRepository criteriaSubResultRepository,
            IPeriodAuditRepository periodAuditRepository,
            IScaleGroupRepository scaleGroupRepository)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _periodAuditTableScaleTemplateResultRepository = periodAuditTableScaleTemplateResultRepository;
            _fieldValuesRepository = fieldValuesRepository;
            _scoringCriteriaRepository = scoringCriteriaRepository;
            _scaleCompanyRepository = scaleCompanyRepository;
            _periodAuditScoringCriteriaResultRepository = periodAuditScoringCriteriaResultRepository;
            _periodAuditScaleSubResultRepository = periodAuditScaleSubResultRepository;
            _periodAuditGroupResultService = periodAuditGroupResultService;
            _storageFilesRepository = storageFilesRepository;
            _storageFilesService = storageFilesService;
            _userReferenceRepository = userReferenceRepository;
            _serviceScopeFactory = serviceScopeFactory;
            _tableScaleTemplateRepository = tableScaleTemplateRepository;
            _auditTemplateFieldRepository = auditTemplateFieldRepository;
            _periodAuditFieldValuesRepository = periodAuditFieldValuesRepository;
            _criteriaSubResultRepository = criteriaSubResultRepository;
            _periodAuditRepository = periodAuditRepository;
            _scaleGroupRepository = scaleGroupRepository;
        }

        public async Task<ResponseDto<PeriodAuditScaleResultResponseDto>> Create(PeriodAuditScaleResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditScaleResultResponseDto>();
            try
            {
                var validate = await _validator.ValidateAsync(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var periodAuditScaleResult = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditGroupResultId == requestDto.PeriodAuditGroupResultId,
                    orderBy: x => x.OrderByDescending(y => y.SortOrder)
                );

                var scaleGroup = await _scaleGroupRepository.GetFirstOrDefaultAsync(filter: x=>x.ScaleGroupId == requestDto.ScaleGroupId && x.IsActive);

                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<PeriodAuditScaleResult>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                entity.IsActive = true;
                entity.AppliedWeighting = requestDto.AppliedWeighting.HasValue && requestDto.AppliedWeighting >= 0 ? requestDto.AppliedWeighting.Value : scaleGroup.Weighting;
                entity.SortOrder = periodAuditScaleResult != null ? periodAuditScaleResult.SortOrder + 1 : 1 ;
                _repository.Insert(entity);
                var scaleGroupResponse = _mapper.Map<ScaleGroupResponseDto>(scaleGroup);
                var periodAuditScaleResultResponse = _mapper.Map<PeriodAuditScaleResultResponseDto>(periodAuditScaleResult);
                await _periodAuditGroupResultService.CreateTableScaleTemplateResults(scaleGroupResponse, periodAuditScaleResultResponse);
                await _unitOfWork.CommitAsync();
                var createdEntity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditScaleResultId == entity.PeriodAuditScaleResultId && x.IsActive,
                    includeProperties: [x => x.PeriodAuditGroupResult, sg => sg.ScaleGroup]);
                response.Data = _mapper.Map<PeriodAuditScaleResultResponseDto>(createdEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditScaleResultId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el registro.");
                    return response;
                }
                var periodAuditGroupResult = await _periodAuditGroupResultService.GetById(entity.PeriodAuditGroupResultId);
                var periodAudit = await _periodAuditRepository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == periodAuditGroupResult.Data.PeriodAuditId, includeProperties: [ x => x.AuditStatus]);
                if (periodAudit != null)
                {
                    if (periodAudit.AuditStatus.Code != AuditStatusCode.Pending)
                    {
                        response = ResponseDto.Error("No se puede eliminar una escala si no está en estado pendiente.");
                        return response;
                    }
                }

                var tableResults = await _periodAuditTableScaleTemplateResultRepository.GetAsync(x => x.PeriodAuditScaleResultId == entity.PeriodAuditScaleResultId && x.IsActive);
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

                var subResults = await _periodAuditScaleSubResultRepository.GetAsync(x => x.PeriodAuditScaleResultId == entity.PeriodAuditScaleResultId && x.IsActive);
                foreach (var subResult in subResults)
                {
                    subResult.IsActive = false;
                    subResult.UpdateDate = DateTime.Now;
                    _periodAuditScaleSubResultRepository.Update(subResult);
                }

                var scoringResults = await _periodAuditScoringCriteriaResultRepository.GetAsync(x => x.PeriodAuditScaleResultId == entity.PeriodAuditScaleResultId && x.IsActive);
                foreach (var scoringResult in scoringResults)
                {
                    scoringResult.IsActive = false;
                    scoringResult.UpdateDate = DateTime.Now;
                    _periodAuditScoringCriteriaResultRepository.Update(scoringResult);
                }


                entity.IsActive = false;
                entity.UpdateDate = DateTime.UtcNow;
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

        public async Task<ResponseDto<PeriodAuditScaleResultResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<PeriodAuditScaleResultResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditScaleResultId == id && x.IsActive,
                    includeProperties: [ x => x.PeriodAuditGroupResult, sg => sg.ScaleGroup]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>("No se encontró el registro.");
                    return response;
                }
                response.Data = _mapper.Map<PeriodAuditScaleResultResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditScaleResultResponseDto>> Update(Guid id, UpdatePeriodAuditScaleResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditScaleResultResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditScaleResultId == id && x.IsActive,
                    includeProperties: [x => x.PeriodAuditGroupResult, sg => sg.ScaleGroup]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>("No se encontró el registro.");
                    return response;
                }
                var periodAuditGroupResult = await _periodAuditGroupResultService.GetById(entity.PeriodAuditGroupResultId);
                var periodAudit = await _periodAuditRepository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == periodAuditGroupResult.Data.PeriodAuditId, includeProperties: [x => x.AuditStatus]);
                if (periodAudit != null)
                {
                    if (periodAudit.AuditStatus.Code != AuditStatusCode.Pending)
                    {
                        response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>("No se puede actualizar una escala si no está en estado pendiente.");
                        return response;
                    }
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity.AppliedWeighting = requestDto.AppliedWeighting;
                entity.UpdateAudit(currentUser.UserName);
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<PeriodAuditScaleResultResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<PeriodAuditScaleResultResponseDto>>> GetPaged(PeriodAuditScaleResultFilterRequestDto filterRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<PeriodAuditScaleResultResponseDto>>();
            try
            {
                Expression<Func<PeriodAuditScaleResult, bool>> filter = x => x.IsActive;
                if (filterRequestDto.PeriodAuditGroupResultId.HasValue)
                {
                    filter = filter.AndAlso(x => x.PeriodAuditGroupResultId == filterRequestDto.PeriodAuditGroupResultId.Value);
                }
                if (filterRequestDto.ScaleGroupId.HasValue)
                {
                    filter = filter.AndAlso(x => x.ScaleGroupId == filterRequestDto.ScaleGroupId.Value);
                }
                if (!string.IsNullOrEmpty(filterRequestDto.Filter))
                {
                    filter = filter.AndAlso(x => (x.Observations != null && x.Observations.Contains(filterRequestDto.Filter)));
                }

                Func<IQueryable<PeriodAuditScaleResult>, IOrderedQueryable<PeriodAuditScaleResult>> orderBy = q => q.OrderBy(x => x.SortOrder);
                var entities = await _repository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: filterRequestDto.PageNumber,
                    pageSize: filterRequestDto.PageSize,
                    includeProperties: [ x => x.ScaleGroup, x => x.PeriodAuditGroupResult ]
                );
                var pagedResult = new PaginationResponseDto<PeriodAuditScaleResultResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<PeriodAuditScaleResultResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = filterRequestDto.PageNumber,
                    PageSize = filterRequestDto.PageSize
                };
                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<PeriodAuditScaleResultResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditScaleResultCustomResponseDto>> GetByIdCustomData(Guid id)
        {
            var response = ResponseDto.Create<PeriodAuditScaleResultCustomResponseDto>();
            try
            {
                var entity = await _repository.GetCustomByIdAsync(filter: x => x.PeriodAuditScaleResultId == id && x.IsActive);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "No se encontro la entidad", MessageType = ApplicationMessageType.Error });
                    return response;
                }
                if (entity.PeriodAuditGroupResult.PeriodAudit.Store.Enterprise.ScaleCompanies == null ||
                    !entity.PeriodAuditGroupResult.PeriodAudit.Store.Enterprise.ScaleCompanies.Any())
                {
                    var defaultScaleCompanies = await _scaleCompanyRepository.GetAsync(filter: e => e.EnterpriseId == null);
                    if (defaultScaleCompanies != null && defaultScaleCompanies.Any())
                    {
                        entity.PeriodAuditGroupResult.PeriodAudit.Store.Enterprise.ScaleCompanies = defaultScaleCompanies.ToList();
                    }
                }
                var fileDataSourceTemplate = (DataSourceFiles?)null;
                var fileDataSource = (DataSourceFiles?)null;
                if (entity.ScaleGroup.HasSourceData == true)
                {
                    var storageFileSourceTemplate = await _storageFilesRepository.GetFirstOrDefaultAsync(
                        filter: x => x.EntityId == entity.ScaleGroupId
                                  && x.ClassificationType == "data_source_template"
                                  && x.IsActive);

                    var storageFileSource = await _storageFilesRepository.GetFirstOrDefaultAsync(
                        filter: x => x.EntityId == entity.PeriodAuditScaleResultId
                                  && x.ClassificationType == "data_source"
                                  && x.IsActive);

                    if (storageFileSourceTemplate != null)
                    {
                        var dataSourceTemplate = await _storageFilesService.GetExcelFile(
                            id: storageFileSourceTemplate.StorageFileId,
                            entityId: null);

                        if (dataSourceTemplate.Data != null)
                        {
                            fileDataSourceTemplate = new DataSourceFiles
                            {
                                FileName = dataSourceTemplate.Data.OriginalName,
                                ClassificationType = dataSourceTemplate.Data.ClassificationType,
                                DataSourceFile = dataSourceTemplate.Data.Base64Result
                            };
                        }
                    }

                    if (storageFileSource != null)
                    {
                        var dataSource = await _storageFilesService.GetExcelFile(
                            id: storageFileSource.StorageFileId,
                            entityId: null);

                        if (dataSource.Data != null)
                        {
                            fileDataSource = new DataSourceFiles
                            {
                                FileName = dataSource.Data.OriginalName,
                                ClassificationType = dataSource.Data.ClassificationType,
                                DataSourceFile = dataSource.Data.Base64Result
                            };
                        }
                    }
                }
                var customDto = _mapper.Map<PeriodAuditScaleResultCustomResponseDto>(entity);
                customDto.ScaleGroup.DataSourceTemplate = fileDataSourceTemplate;
                customDto.ScaleGroup.DataSource = fileDataSource;
                var currentUser = _httpContextAccessor.CurrentUser();
                if (entity.PeriodAuditGroupResult.PeriodAudit.PeriodAuditParticipants.Any(p => p.UserReferenceId == currentUser.UserReferenceId
                 && p.IsActive && p.RoleCodeSnapshot == RoleCodes.Auditor.Code))
                {
                    customDto.PeriodAudit.IAmAuditor = true;
                }
                else
                {
                    customDto.PeriodAudit.IAmAuditor = false;
                }
                response.Data = customDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto<bool>> UpdateAllFieldValues(
            Guid periodAuditScaleResultId,
            PeriodAuditFieldValuesUpdateAllValuesRequestDto periodAuditFieldValuesUpdateAllValuesRequestDto)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                if (periodAuditFieldValuesUpdateAllValuesRequestDto.PeriodAuditScoringCriteriaResultId == Guid.Empty)
                {
                    response.WithMessage("El ID de criterio de puntuación no puede estar vacío", null, ApplicationMessageType.Error);
                    return response;
                }
                var scoringCriteriaResult = await _periodAuditScoringCriteriaResultRepository.GetFirstOrDefaultAsync(
                    x => x.PeriodAuditScoringCriteriaResultId == periodAuditFieldValuesUpdateAllValuesRequestDto.PeriodAuditScoringCriteriaResultId && x.IsActive);
                if (scoringCriteriaResult == null)
                {
                    response.WithMessage("No se encontró el criterio de puntuación para el ID proporcionado", null, ApplicationMessageType.Error);
                    return response;
                }

                var periodAuditScaleResults = await _repository.GetFirstOrDefaultAsync(x => x.PeriodAuditScaleResultId == periodAuditScaleResultId && x.IsActive, includeProperties: [x=>x.PeriodAuditGroupResult.PeriodAudit.AuditStatus]);
                if (periodAuditScaleResults == null)
                {
                    response.WithMessage("No se encontró el resultado del grupo de auditoría para el ID proporcionado", null, ApplicationMessageType.Error);
                    return response;
                }
                if (periodAuditScaleResults.PeriodAuditGroupResult.PeriodAudit.AuditStatus.Code == AuditStatusCode.Pending)
                {
                    response.WithMessage("No se pueden actualizar los valores de campo para una auditoría en estado Pendiente", null, ApplicationMessageType.Error);
                    return response;
                }
                // Guardar el PeriodAuditScoringCriteriaResult
                scoringCriteriaResult.ResultObtained = periodAuditFieldValuesUpdateAllValuesRequestDto.ResultObtained;
                var currentUser = _httpContextAccessor.CurrentUser();
                scoringCriteriaResult.UpdateAudit(currentUser.UserName);
                _periodAuditScoringCriteriaResultRepository.Update(scoringCriteriaResult);

                // Actualizar los PeriodAuditScaleSubResult
                foreach (var subResultDto in periodAuditFieldValuesUpdateAllValuesRequestDto.PeriodAuditScaleSubResult)
                {
                    var subResultEntity = await _periodAuditScaleSubResultRepository.GetFirstOrDefaultAsync(
                        x => x.PeriodAuditScaleSubResultId == subResultDto.PeriodAuditScaleSubResultId && x.IsActive);
                    if (subResultEntity != null)
                    {
                        subResultEntity.ScoreObtained = subResultDto.ScoreObtained;
                        subResultEntity.UpdateAudit(currentUser.UserName);
                        _periodAuditScaleSubResultRepository.Update(subResultEntity);
                    }
                }

                var customResponse = await this.GetByIdCustomData(periodAuditScaleResultId);
                var scaleCompany = await _scaleCompanyRepository.GetAsync(x => x.EnterpriseId == customResponse.Data.PeriodAudit.EnterpriseId && x.IsActive);
                if (scaleCompany == null || !scaleCompany.Any())
                {
                    scaleCompany = await _scaleCompanyRepository.GetAsync(filter: e => e.EnterpriseId == null);
                    if (scaleCompany == null || !scaleCompany.Any())
                    {
                        response = ResponseDto.Error<bool>("No se encontró la escala asociada a la empresa ni la escala por defecto.");
                        return response;
                    }
                }

                periodAuditScaleResults.ScoreValue = scoringCriteriaResult.Score;
                // Actualizar color y descripción de la escala según el nuevo puntaje
                bool foundScale = false;
                foreach (var scale in scaleCompany)
                {
                    if (periodAuditScaleResults.ScoreValue <= scale.MaxValue && periodAuditScaleResults.ScoreValue >= scale.MinValue)
                    {
                        periodAuditScaleResults.ScaleColor = scale.ColorCode;
                        periodAuditScaleResults.ScaleDescription = scale.Name;
                        foundScale = true;
                        break;
                    }
                    
                }
                if (!foundScale)
                {
                    response.WithMessage("No se encontró una escala correspondiente para el puntaje obtenido", null, ApplicationMessageType.Error);
                    return response;
                }
                _repository.Update(periodAuditScaleResults);

                var periodAuditTableScaleTemplateResults = await _periodAuditTableScaleTemplateResultRepository.GetAsync(x=>x.PeriodAuditScaleResultId == periodAuditScaleResults.PeriodAuditScaleResultId && x.IsActive);

                var periodAuditTableScaleTemplateResultIds = periodAuditTableScaleTemplateResults
                    .Select(x => x.PeriodAuditTableScaleTemplateResultId)
                    .ToList();

                // Buscar valores de campo relacionados
                var periodAuditFieldValues = await _fieldValuesRepository.GetAsync(
                    x => periodAuditTableScaleTemplateResultIds.Contains(x.PeriodAuditTableScaleTemplateResultId) && x.IsActive);

                if (periodAuditFieldValues == null || !periodAuditFieldValues.Any())
                {
                    response.WithMessage("No se encontraron valores de campo para actualizar", null, ApplicationMessageType.Error);
                    return response;
                }

                foreach (var fieldValueDto in periodAuditFieldValuesUpdateAllValuesRequestDto.PeriodAuditFieldValues)
                {
                    var fieldValueEntity = periodAuditFieldValues
                        .FirstOrDefault(fv => fv.PeriodAuditFieldValueId == fieldValueDto.PeriodAuditFieldValueId);
                    fieldValueDto.SortOrder = fieldValueEntity.SortOrder;
                    fieldValueDto.DefaultValue = fieldValueEntity.DefaultValue;
                    if (fieldValueEntity == null)
                        continue;
                    _mapper.Map(fieldValueDto, fieldValueEntity);
                    _fieldValuesRepository.Update(fieldValueEntity);
                }
                await _unitOfWork.CommitAsync();

                await using (var scope = _serviceScopeFactory.CreateAsyncScope())
                {
                    var groupResultService = scope.ServiceProvider.GetRequiredService<IPeriodAuditGroupResultService>();
                    await groupResultService.Recalculate(periodAuditScaleResults.PeriodAuditGroupResultId);
                }

                response.Data = true;
                response.WithMessage("Actualización completada correctamente.", null, ApplicationMessageType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error al actualizar valores de campo");

                response.Data = false;
                response.Messages.Add(new ApplicationMessage
                {
                    Message = ex.Message,
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }
    }
}
