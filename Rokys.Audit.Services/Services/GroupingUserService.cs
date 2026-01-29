using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.GroupingUser;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.EmployeeStore;
using Rokys.Audit.DTOs.Responses.GroupingUser;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Services.Interfaces.Validations;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class GroupingUserService : IGroupingUserService
    {
        private readonly IGroupingUserRepository _groupingUserRepository;
        private readonly IValidator<GroupingUserRequestDto> _fluentValidator;
        private readonly ILogger<GroupingUserService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupingUserService(
            IGroupingUserRepository groupingUserRepository,
            IValidator<GroupingUserRequestDto> idValidator,
            ILogger<GroupingUserService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _groupingUserRepository = groupingUserRepository;
            _fluentValidator = idValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<GroupingUserResponseDto>> Create(GroupingUserRequestDto requestDto)
        {
            var response = ResponseDto.Create<GroupingUserResponseDto>();
            try
            {
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<GroupingUser>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                _groupingUserRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<GroupingUserResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrio un error al intentar crear el grupo de usuario.");
                response = ResponseDto.Error<GroupingUserResponseDto>("Ocurrio un error al intentar crear el grupo de usuario.");
            }
            return response;
        }
        public async Task<ResponseDto<GroupingUserResponseDto>> Update(Guid id, GroupingUserRequestDto requestDto)
        {
            var response = ResponseDto.Create<GroupingUserResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _groupingUserRepository.GetFirstOrDefaultAsync(filter: x => x.GroupingUserId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<GroupingUserResponseDto>("El grupo de usuario no existe.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _groupingUserRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<GroupingUserResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrio un error al intentar actualizar el grupo de usuario.");
                response = ResponseDto.Error<GroupingUserResponseDto>("Ocurrio un error al intentar actualizar el grupo de usuario.");
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _groupingUserRepository.GetFirstOrDefaultAsync(filter: x => x.GroupingUserId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("El grupo de usuario no existe.");
                    return response;
                }
                entity.IsActive = false;
                _groupingUserRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrio un error al intentar eliminar el grupo de usuario.");
                response = ResponseDto.Error("Ocurrio un error al intentar eliminar el grupo de usuario.");
            }
            return response;
        }

        public async Task<ResponseDto<GroupingUserResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<GroupingUserResponseDto>();
            try
            {
                var entity = await _groupingUserRepository.GetFirstOrDefaultAsync(filter: x => x.GroupingUserId == id && x.IsActive, includeProperties: [x => x.EnterpriseGrouping, e => e.UserReference]);
                if (entity == null)
                {
                    response = ResponseDto.Error<GroupingUserResponseDto>("El grupo de usuario no existe.");
                    return response;
                }
                response.Data = _mapper.Map<GroupingUserResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrio un error al intentar traer el grupo de usuario.");
                response = ResponseDto.Error<GroupingUserResponseDto>("Ocurrio un error al intentar traer el grupo de usuario.");
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<GroupingUserResponseDto>>> GetPaged(GroupingUserFilterRequestDto filterRequest)
        {
            var response = ResponseDto.Create<PaginationResponseDto<GroupingUserResponseDto>>();
            try
            {
                Expression<Func<GroupingUser, bool>> filter = x => x.IsActive;

                Func<IQueryable<GroupingUser>, IOrderedQueryable<GroupingUser>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _groupingUserRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: filterRequest.PageNumber,
                    pageSize: filterRequest.PageSize,
                    includeProperties: [x => x.EnterpriseGrouping, e => e.UserReference]
                 );

                var pagedResult = new PaginationResponseDto<GroupingUserResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<GroupingUserResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = filterRequest.PageNumber,
                    PageSize = filterRequest.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrio un error al intentar traer el grupo de usuarios.");
                response = ResponseDto.Error<PaginationResponseDto<GroupingUserResponseDto>>("Ocurrio un error al intentar traer el grupo de usuarios.");
            }
            return response;
        }
    }
}
