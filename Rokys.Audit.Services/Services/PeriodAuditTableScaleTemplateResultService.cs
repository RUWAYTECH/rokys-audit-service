using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rokys.Audit.DTOs.Requests.PeriodAuditTableScaleTemplateResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditTableScaleTemplateResult;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;
using Newtonsoft.Json;

namespace Rokys.Audit.Services.Services
{
    public class PeriodAuditTableScaleTemplateResultService : IPeriodAuditTableScaleTemplateResultService
    {
        private readonly IPeriodAuditTableScaleTemplateResultRepository _repository;
        private readonly IValidator<PeriodAuditTableScaleTemplateResultRequestDto> _validator;
        private readonly ILogger<PeriodAuditTableScaleTemplateResultService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPeriodAuditFieldValuesRepository _fieldValuesRepository;
        private readonly IPeriodAuditScaleResultRepository _periodAuditScaleResultRepository;

        public PeriodAuditTableScaleTemplateResultService(
            IPeriodAuditTableScaleTemplateResultRepository repository,
            IValidator<PeriodAuditTableScaleTemplateResultRequestDto> validator,
            ILogger<PeriodAuditTableScaleTemplateResultService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IPeriodAuditFieldValuesRepository periodAuditFieldValuesRepository,
            IPeriodAuditScaleResultRepository periodAuditScaleResultRepository)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _fieldValuesRepository = periodAuditFieldValuesRepository;
            _periodAuditScaleResultRepository = periodAuditScaleResultRepository;
        }

        public async Task<ResponseDto<PeriodAuditTableScaleTemplateResultResponseDto>> Create(PeriodAuditTableScaleTemplateResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditTableScaleTemplateResultResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
                var entity = _mapper.Map<PeriodAuditTableScaleTemplateResult>(requestDto);
                entity.CreatedBy = currentUser;
                entity.CreationDate = DateTime.UtcNow;
                _repository.Insert(entity);
                await _unitOfWork.CommitAsync();
                var createdEntity = await _repository.GetFirstOrDefaultAsync(x => x.PeriodAuditTableScaleTemplateResultId == entity.PeriodAuditTableScaleTemplateResultId);
                response.Data = _mapper.Map<PeriodAuditTableScaleTemplateResultResponseDto>(createdEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PeriodAuditTableScaleTemplateResult");
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditTableScaleTemplateResultResponseDto>> Update(Guid id, PeriodAuditTableScaleTemplateResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditTableScaleTemplateResultResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(x => x.PeriodAuditTableScaleTemplateResultId == id);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "Entity not found", MessageType = ApplicationMessageType.Error });
                    return response;
                }
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
                _mapper.Map(requestDto, entity);
                entity.UpdatedBy = currentUser;
                entity.UpdateDate = DateTime.UtcNow;
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<PeriodAuditTableScaleTemplateResultResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating PeriodAuditTableScaleTemplateResult");
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto<bool>> Delete(Guid id)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(x => x.PeriodAuditTableScaleTemplateResultId == id);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "Entity not found", MessageType = ApplicationMessageType.Error });
                    response.Data = false;
                    return response;
                }
                _repository.Delete(entity);
                await _unitOfWork.CommitAsync();
                response.Data = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PeriodAuditTableScaleTemplateResult");
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
                response.Data = false;
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditTableScaleTemplateResultResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<PeriodAuditTableScaleTemplateResultResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(x => x.PeriodAuditTableScaleTemplateResultId == id);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "Entity not found", MessageType = ApplicationMessageType.Error });
                    return response;
                }
                response.Data = _mapper.Map<PeriodAuditTableScaleTemplateResultResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PeriodAuditTableScaleTemplateResult by id");
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto<List<PeriodAuditTableScaleTemplateResultResponseDto>>> GetAll()
        {
            var response = ResponseDto.Create<List<PeriodAuditTableScaleTemplateResultResponseDto>>();
            try
            {
                var entities = await _repository.GetAsync();
                response.Data = _mapper.Map<List<PeriodAuditTableScaleTemplateResultResponseDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all PeriodAuditTableScaleTemplateResults");
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto<List<PeriodAuditTableScaleTemplateResultResponseDto>>> Filter(PeriodAuditTableScaleTemplateResultFilterRequestDto filter)
        {
            var response = ResponseDto.Create<List<PeriodAuditTableScaleTemplateResultResponseDto>>();
            try
            {
                var entities = await _repository.GetAsync(x =>
                    (!filter.PeriodAuditScaleResultId.HasValue || x.PeriodAuditScaleResultId == filter.PeriodAuditScaleResultId.Value) &&
                    (!filter.TableScaleTemplateId.HasValue || x.TableScaleTemplateId == filter.TableScaleTemplateId.Value) &&
                    (string.IsNullOrEmpty(filter.Code) || x.Code == filter.Code) &&
                    (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                    (!filter.IsActive.HasValue || x.IsActive == filter.IsActive.Value)
                );
                response.Data = _mapper.Map<List<PeriodAuditTableScaleTemplateResultResponseDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering PeriodAuditTableScaleTemplateResults");
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto<List<PeriodAuditTableScaleTemplateResultListResponseDto>>> GetByPeriodAuditScaleResult(Guid periodAuditScaleResultId)
        {
            var response = ResponseDto.Create<List<PeriodAuditTableScaleTemplateResultListResponseDto>>();
            try
            {
                var entities = await _repository.GetByPeriodAuditScaleResultId(periodAuditScaleResultId);
                if (entities == null)
                {
                    response.WithMessage("No se encontro resultados", null, ApplicationMessageType.Error);
                    return response;
                }
                var periodAuditTableScaleTemplateIds = entities
                    .Select(e => e.PeriodAuditTableScaleTemplateResultId)
                    .ToList();

                var periodAuditFieldValues = await _fieldValuesRepository.GetAsync(
                    x => periodAuditTableScaleTemplateIds.Contains(x.PeriodAuditTableScaleTemplateResultId)
                );

                foreach (var entity in entities)
                {
                    entity.PeriodAuditFieldValues = periodAuditFieldValues
                        .Where(fv => fv.PeriodAuditTableScaleTemplateResultId == entity.PeriodAuditTableScaleTemplateResultId)
                        .ToList();
                }
                response.Data = _mapper.Map<List<PeriodAuditTableScaleTemplateResultListResponseDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }
    }
}
