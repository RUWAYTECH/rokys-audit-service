using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.PeriodAuditFieldValues;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rokys.Audit.Services.Services
{
    public class PeriodAuditFieldValuesService : IPeriodAuditFieldValuesService
    {
        private readonly IRepository<PeriodAuditFieldValues> _repository;
        private readonly IValidator<PeriodAuditFieldValuesRequestDto> _validator;
        private readonly ILogger<PeriodAuditFieldValuesService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PeriodAuditFieldValuesService(
            IRepository<PeriodAuditFieldValues> repository,
            IValidator<PeriodAuditFieldValuesRequestDto> validator,
            ILogger<PeriodAuditFieldValuesService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<PeriodAuditFieldValuesResponseDto>> Create(PeriodAuditFieldValuesRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditFieldValuesResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<PeriodAuditFieldValues>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                _repository.Insert(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<PeriodAuditFieldValuesResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditFieldValuesResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditFieldValueId == id && x.IsActive);
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

        public async Task<ResponseDto<PeriodAuditFieldValuesResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<PeriodAuditFieldValuesResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditFieldValueId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditFieldValuesResponseDto>("No se encontró el registro.");
                    return response;
                }
                response.Data = _mapper.Map<PeriodAuditFieldValuesResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditFieldValuesResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditFieldValuesResponseDto>> Update(Guid id, PeriodAuditFieldValuesRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditFieldValuesResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditFieldValueId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditFieldValuesResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<PeriodAuditFieldValuesResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditFieldValuesResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<PeriodAuditFieldValuesResponseDto>>> GetPaged(PaginationRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<PeriodAuditFieldValuesResponseDto>>();
            try
            {
                Expression<Func<PeriodAuditFieldValues, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = filter.AndAlso(x => x.FieldName.Contains(paginationRequestDto.Filter));

                Func<IQueryable<PeriodAuditFieldValues>, IOrderedQueryable<PeriodAuditFieldValues>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _repository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize
                );

                var pagedResult = new PaginationResponseDto<PeriodAuditFieldValuesResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<PeriodAuditFieldValuesResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<PeriodAuditFieldValuesResponseDto>>(ex.Message);
            }
            return response;
        }
    }
}
