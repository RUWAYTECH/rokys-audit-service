using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Group;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Group;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IValidator<GroupRequestDto> _fluentValidator;
        private readonly ILogger<GroupService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupService(
            IGroupRepository groupRepository,
            IValidator<GroupRequestDto> fluentValidator,
            ILogger<GroupService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _groupRepository = groupRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<GroupResponseDto>> Create(GroupRequestDto requestDto)
        {
            var response = ResponseDto.Create<GroupResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<Group>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                _groupRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                var createResponse = await _groupRepository.GetFirstOrDefaultAsync(filter: x => x.GroupId == entity.GroupId, includeProperties: [x => x.Enterprise]);
                response.Data = _mapper.Map<GroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<GroupResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _groupRepository.GetFirstOrDefaultAsync(filter: x => x.GroupId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el grupo.");
                    return response;
                }
                entity.IsActive = false;
                _groupRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<GroupResponseDto>>> GetPaged(GroupFilterRequestDto groupFilterRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<GroupResponseDto>>();
            try
            {
                var filter = BuildFilter(groupFilterRequestDto);

                Func<IQueryable<Group>, IOrderedQueryable<Group>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _groupRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: groupFilterRequestDto.PageNumber,
                    pageSize: groupFilterRequestDto.PageSize,
                    includeProperties: [ x => x.Enterprise]
                );

                var pagedResult = new PaginationResponseDto<GroupResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<GroupResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = groupFilterRequestDto.PageNumber,
                    PageSize = groupFilterRequestDto.PageSize
                };

                response.Data = pagedResult;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<GroupResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<GroupResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<GroupResponseDto>();
            try
            {
                var entity = await _groupRepository.GetFirstOrDefaultAsync(filter: x => x.GroupId == id && x.IsActive, includeProperties: [x => x.Enterprise]);
                if (entity == null)
                {
                    response = ResponseDto.Error<GroupResponseDto>("No se encontró el grupo.");
                    return response;
                }
                response.Data = _mapper.Map<GroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<GroupResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<GroupResponseDto>> Update(Guid id, GroupRequestDto requestDto)
        {
            var response = ResponseDto.Create<GroupResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _groupRepository.GetFirstOrDefaultAsync(filter: x => x.GroupId == id && x.IsActive, includeProperties: [x => x.Enterprise]);
                if (entity == null)
                {
                    response = ResponseDto.Error<GroupResponseDto>("No se encontró el grupo.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _groupRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<GroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<GroupResponseDto>(ex.Message);
            }
            return response;
        }

        private Expression<Func<Group, bool>> BuildFilter(GroupFilterRequestDto dto)
        {
            return x =>
                x.IsActive &&
                (!dto.EnterpriseId.HasValue || x.EnterpriseId == dto.EnterpriseId.Value) &&
                (string.IsNullOrEmpty(dto.Filter) || x.Name.Contains(dto.Filter));
        }
    }
}