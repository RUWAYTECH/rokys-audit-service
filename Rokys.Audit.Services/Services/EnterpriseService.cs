using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Enterprise;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Enterprise;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class EnterpriseService : IEnterpriseService
    {
        private readonly IEnterpriseRepository _enterpriseRepository;
        private readonly IValidator<EnterpriseRequestDto> _fluentValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EnterpriseService(
            IEnterpriseRepository enterpriseRepository,
            IValidator<EnterpriseRequestDto> fluentValidator,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            ILogger<EnterpriseService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _enterpriseRepository = enterpriseRepository;
            _fluentValidator = fluentValidator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<EnterpriseResponseDto>> Create(EnterpriseRequestDto requestDto)
        {
            var response = ResponseDto.Create<EnterpriseResponseDto>();
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
                    var entity = _mapper.Map<Enterprise>(requestDto);
                    entity.CreateAudit(currentUser.UserName);
                    _enterpriseRepository.Insert(entity);
                    await _unitOfWork.CommitAsync();
                    response.Data = _mapper.Map<EnterpriseResponseDto>(entity);
                }
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<EnterpriseResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public Task<ResponseDto> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<EnterpriseResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<EnterpriseResponseDto>();
            try
            {
                var entity = await _enterpriseRepository.GetFirstOrDefaultAsync(filter: x => x.EnterpriseId == id && x.IsActive);
                response.Data = _mapper.Map<EnterpriseResponseDto>(entity);
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<EnterpriseResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<EnterpriseResponseDto>>> GetPaged(EnterpriseFilterRequestDto requestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<EnterpriseResponseDto>>();
            try
            {
                int totalRows;
                Expression<Func<Enterprise, bool>> filter = x => x.IsActive;

                if (!string.IsNullOrEmpty(requestDto.Filter))
                    filter = x => x.Name.Contains(requestDto.Filter);

                if (requestDto.StartDate.HasValue)
                    filter = filter.AndAlso(x => x.CreationDate >= requestDto.StartDate.Value);

                if (requestDto.EndDate.HasValue)
                    filter = filter.AndAlso(x => x.CreationDate <= requestDto.EndDate.Value);

                Func<IQueryable<Enterprise>, IOrderedQueryable<Enterprise>> orderBy = q => q.OrderBy(x => x.Name);

                var entities = await _enterpriseRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: requestDto.PageNumber,
                    pageSize: requestDto.PageSize
                );

                var pagedResult = new PaginationResponseDto<EnterpriseResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<EnterpriseResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = requestDto.PageNumber,
                    PageSize = requestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<PaginationResponseDto<EnterpriseResponseDto>>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public Task<ResponseDto<EnterpriseResponseDto>> Update(Guid id, EnterpriseRequestDto requestDto)
        {
            throw new NotImplementedException();
        }
    }
}
