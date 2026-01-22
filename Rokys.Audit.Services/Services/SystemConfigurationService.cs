using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.SystemConfiguration;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.SystemConfiguration;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class SystemConfigurationService : ISystemConfigurationService
    {
        private readonly ISystemConfigurationRepository _systemConfigurationRepository;
        private readonly IValidator<SystemConfigurationRequestDto> _fluentValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SystemConfigurationService(
            ISystemConfigurationRepository systemConfigurationRepository,
            IValidator<SystemConfigurationRequestDto> fluentValidator,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            ILogger<SystemConfigurationService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _systemConfigurationRepository = systemConfigurationRepository;
            _fluentValidator = fluentValidator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<SystemConfigurationResponseDto>> Create(SystemConfigurationRequestDto requestDto)
        {
            var response = ResponseDto.Create<SystemConfigurationResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                else
                {
                    var currentUser = _httpContextAccessor.CurrentUser();
                    var entity = _mapper.Map<SystemConfiguration>(requestDto);
                    entity.CreateAudit(currentUser.UserName);
                    _systemConfigurationRepository.Insert(entity);
                    await _unitOfWork.CommitAsync();
                    response.Data = _mapper.Map<SystemConfigurationResponseDto>(entity);
                }
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<SystemConfigurationResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public Task<ResponseDto> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<SystemConfigurationResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<SystemConfigurationResponseDto>();
            try
            {
                var entity = await _systemConfigurationRepository.GetFirstOrDefaultAsync(filter: x => x.SystemConfigurationId == id && x.IsActive);
                response.Data = _mapper.Map<SystemConfigurationResponseDto>(entity);
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<SystemConfigurationResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<SystemConfigurationResponseDto>>> GetPaged(SystemConfigurationFilterRequestDto requestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<SystemConfigurationResponseDto>>();
            try
            {
                Expression<Func<SystemConfiguration, bool>> filter = x => x.IsActive;

                if (!string.IsNullOrEmpty(requestDto.Filter))
                    filter = x => x.ConfigKey.Contains(requestDto.Filter) || x.Description.Contains(requestDto.Filter);

                if (!string.IsNullOrEmpty(requestDto.ReferenceType))
                    filter = filter.AndAlso(x => x.ReferenceType == requestDto.ReferenceType);

                if (!string.IsNullOrEmpty(requestDto.DataType))
                    filter = filter.AndAlso(x => x.DataType == requestDto.DataType);

                if (requestDto.StartDate.HasValue)
                    filter = filter.AndAlso(x => x.CreationDate >= requestDto.StartDate.Value);

                if (requestDto.EndDate.HasValue)
                    filter = filter.AndAlso(x => x.CreationDate <= requestDto.EndDate.Value);

                Func<IQueryable<SystemConfiguration>, IOrderedQueryable<SystemConfiguration>> orderBy = q => q.OrderBy(x => x.ConfigKey);

                var entities = await _systemConfigurationRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: requestDto.PageNumber,
                    pageSize: requestDto.PageSize
                );

                var pagedResult = new PaginationResponseDto<SystemConfigurationResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<SystemConfigurationResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = requestDto.PageNumber,
                    PageSize = requestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<PaginationResponseDto<SystemConfigurationResponseDto>>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public Task<ResponseDto<SystemConfigurationResponseDto>> Update(Guid id, SystemConfigurationRequestDto requestDto)
        {
            throw new NotImplementedException();
        }
    }
}
