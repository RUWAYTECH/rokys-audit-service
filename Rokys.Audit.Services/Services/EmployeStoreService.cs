using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.EmployeeStore;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.EmployeeStore;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class EmployeStoreService : IEmployeeStoreService
    {
        private readonly IEmployeeStoreRepository _employeeStoreRepository;
        private readonly IValidator<EmployeeStoreRequestDto> _fluentValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeStoreService(IEmployeeStoreRepository employeeStoreRepository,
            IValidator<EmployeeStoreRequestDto> fluentValidator,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            ILogger<EmployeStoreService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _employeeStoreRepository = employeeStoreRepository;
            _fluentValidator = fluentValidator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<EmployeeStoreResponseDto>> Create(EmployeeStoreRequestDto requestDto)
        {
            var response = ResponseDto.Create<EmployeeStoreResponseDto>();
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
                    var entity = _mapper.Map<EmployeeStore>(requestDto);
                    entity.CreateAudit(currentUser.UserName);
                    _employeeStoreRepository.Insert(entity);
                    await _unitOfWork.CommitAsync();
                    response.Data = _mapper.Map<EmployeeStoreResponseDto>(entity);
                }
            }
            catch (Exception ex)
            {
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _employeeStoreRepository.GetFirstOrDefaultAsync(filter: x => x.EmployeeStoreId == id && x.IsActive);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "No se encontro la tienda del usuario.", MessageType = ApplicationMessageType.Error });
                    return response;
                }
                entity.IsActive = false;
                _employeeStoreRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto<EmployeeStoreResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<EmployeeStoreResponseDto>();
            try
            {
                var entity = await _employeeStoreRepository.GetFirstOrDefaultAsync(filter: x => x.EmployeeStoreId == id && x.IsActive, includeProperties: [ ur => ur.UserReference, s => s.Store ]);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "No se encontro la tienda del usuario.", MessageType = ApplicationMessageType.Error });
                    return response;
                }
                response.Data = _mapper.Map<EmployeeStoreResponseDto>(entity);
            }
            catch (Exception ex)
            {
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto<List<EmployeeStoreResponseDto>>> GetByUserReferenceId(Guid UserReferenceId)
        {
            var response = ResponseDto.Create<List<EmployeeStoreResponseDto>>();
            try
            {
                var entities = _employeeStoreRepository.GetByUserReferenceIdAsync(UserReferenceId);
                response.Data = _mapper.Map<List<EmployeeStoreResponseDto>>(entities.Result);
            }
            catch (Exception ex)
            {
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<EmployeeStoreResponseDto>>> GetPaged(PaginationRequestDto requestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<EmployeeStoreResponseDto>>();
            try
            {
                Expression<Func<EmployeeStore, bool>> filter = x => x.IsActive;

                Func<IQueryable<EmployeeStore>, IOrderedQueryable<EmployeeStore>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _employeeStoreRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: requestDto.PageNumber,
                    pageSize: requestDto.PageSize);

                var pagedResult = new PaginationResponseDto<EmployeeStoreResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<EmployeeStoreResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = requestDto.PageNumber,
                    PageSize = requestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }

        public async Task<ResponseDto<EmployeeStoreResponseDto>> Update(Guid id, EmployeeStoreRequestDto requestDto)
        {
            var response = ResponseDto.Create<EmployeeStoreResponseDto>();
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
                    var entity = await _employeeStoreRepository.GetFirstOrDefaultAsync(filter: x => x.EmployeeStoreId == id && x.IsActive);
                    if (entity == null)
                    {
                        response.Messages.Add(new ApplicationMessage { Message = "No se encontro la tienda del usuario.", MessageType = ApplicationMessageType.Error });
                        return response;
                    }
                    var currentUser = _httpContextAccessor.CurrentUser();
                    entity = _mapper.Map(requestDto, entity);
                    entity.UpdateAudit(currentUser.UserName);
                    _employeeStoreRepository.Update(entity);
                    await _unitOfWork.CommitAsync();
                    response.Data = _mapper.Map<EmployeeStoreResponseDto>(entity);
                }
            }
            catch (Exception ex)
            {
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }
    }
}
