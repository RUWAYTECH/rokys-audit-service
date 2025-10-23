using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;
using Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;
using System.Text.Json;

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
            IPeriodAuditGroupResultService periodAuditGroupResultService)
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
        }

        public async Task<ResponseDto<PeriodAuditScaleResultResponseDto>> Create(PeriodAuditScaleResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditScaleResultResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<PeriodAuditScaleResult>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                entity.IsActive = true;
                _repository.Insert(entity);
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

        public async Task<ResponseDto<PeriodAuditScaleResultResponseDto>> Update(Guid id, PeriodAuditScaleResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditScaleResultResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditScaleResultId == id && x.IsActive,
                    includeProperties: [x => x.PeriodAuditGroupResult, sg => sg.ScaleGroup]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
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
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditScaleResultId == id, 
                    includeProperties: [ e => e.PeriodAuditGroupResult.PeriodAudit.Store.Enterprise.ScaleCompanies,
                        sg => sg.ScaleGroup,
                        a => a.PeriodAuditGroupResult.PeriodAudit.Administrator,
                        op => op.PeriodAuditGroupResult.PeriodAudit.OperationManager,
                        fa => fa.PeriodAuditGroupResult.PeriodAudit.FloatingAdministrator,
                        ra => ra.PeriodAuditGroupResult.PeriodAudit.ResponsibleAuditor,
                        asi => asi.PeriodAuditGroupResult.PeriodAudit.Assistant,
                        su => su.PeriodAuditGroupResult.PeriodAudit.Supervisor,
                        pas => pas.PeriodAuditScaleSubResults,
                        pasc => pasc.PeriodAuditScoringCriteriaResults]);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "No se encontro la entidad", MessageType = ApplicationMessageType.Error });
                    return response;
                }

                var customDto = _mapper.Map<PeriodAuditScaleResultCustomResponseDto>(entity);
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

                var periodAuditScaleResults = await _repository.GetFirstOrDefaultAsync(x => x.PeriodAuditScaleResultId == periodAuditScaleResultId && x.IsActive);
                if (periodAuditScaleResults == null)
                {
                    response.WithMessage("No se encontró el resultado del grupo de auditoría para el ID proporcionado", null, ApplicationMessageType.Error);
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

                await _periodAuditGroupResultService.Recalculate(periodAuditScaleResults.PeriodAuditGroupResultId);

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
