using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.PeriodAuditGroupResult;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class PeriodAuditGroupResultService : IPeriodAuditGroupResultService
    {
        private readonly IRepository<PeriodAuditGroupResult> _repository;
        private readonly IValidator<PeriodAuditGroupResultRequestDto> _validator;
        private readonly ILogger<PeriodAuditGroupResultService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PeriodAuditGroupResultService(
            IRepository<PeriodAuditGroupResult> repository,
            IValidator<PeriodAuditGroupResultRequestDto> validator,
            ILogger<PeriodAuditGroupResultService> logger,
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

        private Expression<Func<PeriodAuditGroupResult, bool>> BuildFilter(PeriodAuditGroupResultFilterRequestDto filterRequestDto)
        {
            Expression<Func<PeriodAuditGroupResult, bool>> filter = x => x.IsActive;
            if (filterRequestDto.PeriodAuditId.HasValue)
            {
                var prevFilter = filter;
                filter = x => prevFilter.Compile().Invoke(x) && x.PeriodAuditId == filterRequestDto.PeriodAuditId.Value;
            }
            if (filterRequestDto.GroupId.HasValue)
            {
                var prevFilter = filter;
                filter = x => prevFilter.Compile().Invoke(x) && x.GroupId == filterRequestDto.GroupId.Value;
            }
            if (!string.IsNullOrEmpty(filterRequestDto.Filter))
            {
                var prevFilter = filter;
                filter = x => prevFilter.Compile().Invoke(x) && (x.ScaleDescription.Contains(filterRequestDto.Filter) || x.Observations.Contains(filterRequestDto.Filter));
            }
            return filter;
        }

        public async Task<ResponseDto<PeriodAuditGroupResultResponseDto>> Create(PeriodAuditGroupResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditGroupResultResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<PeriodAuditGroupResult>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                entity.IsActive = true;
                _repository.Insert(entity);
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

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditGroupResultId == id && x.IsActive);
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

        public async Task<ResponseDto<PeriodAuditGroupResultResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<PeriodAuditGroupResultResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditGroupResultId == id && x.IsActive);
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

        public async Task<ResponseDto<PeriodAuditGroupResultResponseDto>> Update(Guid id, PeriodAuditGroupResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditGroupResultResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditGroupResultId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditGroupResultResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _repository.Update(entity);
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
                var filter = BuildFilter(filterRequestDto);
                Func<IQueryable<PeriodAuditGroupResult>, IOrderedQueryable<PeriodAuditGroupResult>> orderBy = q => q.OrderByDescending(x => x.CreationDate);
                var entities = await _repository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: filterRequestDto.PageNumber,
                    pageSize: filterRequestDto.PageSize
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
    }
}
