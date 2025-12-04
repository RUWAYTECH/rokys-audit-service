using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Store;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Store;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IValidator<StoreRequestDto> _fluentValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StoreService(IStoreRepository storeRepository,
            IValidator<StoreRequestDto> fluentValidator,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            ILogger<StoreService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _storeRepository = storeRepository;
            _fluentValidator = fluentValidator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<StoreResponseDto>> Create(StoreRequestDto requestDto)
        {
            var response = ResponseDto.Create<StoreResponseDto>();
            try
            {
                var validate = await _fluentValidator.ValidateAsync(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                else
                {
                    var currentUser = _httpContextAccessor.CurrentUser();
                    var entity = _mapper.Map<Stores>(requestDto);
                    entity.CreateAudit(currentUser.UserName);
                    _storeRepository.Insert(entity);
                    await _unitOfWork.CommitAsync();
                    response.Data = _mapper.Map<StoreResponseDto>(entity);
                }
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<StoreResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public Task<ResponseDto> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<StoreResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<StoreResponseDto>();
            try
            {
                var entity = await _storeRepository.GetFirstOrDefaultAsync(a => a.StoreId == id && a.IsActive);
                response.Data = _mapper.Map<StoreResponseDto>(entity);
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<StoreResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<StoreResponseDto>>> GetPaged(StoreFilterRequestDto requestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<StoreResponseDto>>();
            try
            {
                Expression<Func<Stores, bool>> filter = x => x.IsActive;

                Func<IQueryable<Stores>, IOrderedQueryable<Stores>> orderBy = q => q.OrderBy(x => x.Name);

                if (!string.IsNullOrEmpty(requestDto.Filter))
                    filter = filter.AndAlso(x => x.Name.Contains(requestDto.Filter) || x.Enterprise.Name.Contains(requestDto.Filter));

                if (requestDto.EnterpriseId != null)
                {
                    var enterpriseGuid = requestDto.EnterpriseId.Split(',').Select(id => Guid.Parse(id.Trim())).ToList();
                    filter = filter.AndAlso(x => enterpriseGuid.Contains(x.EnterpriseId));
                }
                    

                var result = await _storeRepository.GetPagedAsync(
                    filter,
                    orderBy,
                    requestDto.PageNumber,
                    requestDto.PageSize,
                    includeProperties: [x => x.Enterprise]
                );
                response.Data = new PaginationResponseDto<StoreResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<StoreResponseDto>>(result.Items),
                    TotalCount = result.TotalRows,
                    PageNumber = requestDto.PageNumber,
                    PageSize = requestDto.PageSize
                };
            } catch (Exception ex)
            {
                response = ResponseDto.Error<PaginationResponseDto<StoreResponseDto>>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

public Task<ResponseDto<StoreResponseDto>> Update(Guid id, StoreRequestDto requestDto)
        {
            throw new NotImplementedException();
        }
    }
}
