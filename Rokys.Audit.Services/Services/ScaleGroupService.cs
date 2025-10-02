using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScaleGroup;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.ScaleGroup;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class ScaleGroupService : IScaleGroupService
    {
        private readonly IScaleGroupRepository _scaleGroupRepository;
        private readonly IValidator<ScaleGroupRequestDto> _fluentValidator;
        private readonly ILogger<ScaleGroupService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScaleGroupService(
            IScaleGroupRepository scaleGroupRepository,
            IValidator<ScaleGroupRequestDto> fluentValidator,
            ILogger<ScaleGroupService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _scaleGroupRepository = scaleGroupRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<ScaleGroupResponseDto>> Create(ScaleGroupRequestDto requestDto)
        {
            var response = ResponseDto.Create<ScaleGroupResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                // Verificar si ya existe un código duplicado
                var existsByCode = await _scaleGroupRepository.ExistsByCodeAsync(requestDto.Code);
                if (existsByCode)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "Ya existe un grupo de escala con este código.", MessageType = ApplicationMessageType.Error });
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<ScaleGroup>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                _scaleGroupRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<ScaleGroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<ScaleGroupResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _scaleGroupRepository.GetFirstOrDefaultAsync(filter: x => x.ScaleGroupId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el grupo de escala.");
                    return response;
                }
                entity.IsActive = false;
                var currentUser = _httpContextAccessor.CurrentUser();
                entity.UpdateAudit(currentUser.UserName);
                _scaleGroupRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<ScaleGroupResponseDto>>> GetPaged(PaginationRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<ScaleGroupResponseDto>>();
            try
            {
                Expression<Func<ScaleGroup, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = x => x.IsActive && (x.Name.Contains(paginationRequestDto.Filter) || x.Code.Contains(paginationRequestDto.Filter));

                Func<IQueryable<ScaleGroup>, IOrderedQueryable<ScaleGroup>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _scaleGroupRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize,
                    includeProperties: [a => a.Group]
                );

                var pagedResult = new PaginationResponseDto<ScaleGroupResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<ScaleGroupResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<ScaleGroupResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<ScaleGroupResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<ScaleGroupResponseDto>();
            try
            {
                var entity = await _scaleGroupRepository.GetFirstOrDefaultAsync(
                    filter: x => x.ScaleGroupId == id && x.IsActive,
                   includeProperties: [a => a.Group]
                );
                if (entity == null)
                {
                    response = ResponseDto.Error<ScaleGroupResponseDto>("No se encontró el grupo de escala.");
                    return response;
                }
                response.Data = _mapper.Map<ScaleGroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<ScaleGroupResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<IEnumerable<ScaleGroupResponseDto>>> GetByGroupId(Guid groupId)
        {
            var response = ResponseDto.Create<IEnumerable<ScaleGroupResponseDto>>();
            try
            {
                var entities = await _scaleGroupRepository.GetAsync(
                    filter: x => x.GroupId == groupId && x.IsActive,
                    orderBy: q => q.OrderBy(x => x.Name),
                    includeProperties: [a => a.Group]
                );
                response.Data = _mapper.Map<IEnumerable<ScaleGroupResponseDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<IEnumerable<ScaleGroupResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<ScaleGroupResponseDto>> Update(Guid id, ScaleGroupRequestDto requestDto)
        {
            var response = ResponseDto.Create<ScaleGroupResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                var entity = await _scaleGroupRepository.GetFirstOrDefaultAsync(filter: x => x.ScaleGroupId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<ScaleGroupResponseDto>("No se encontró el grupo de escala.");
                    return response;
                }

                // Verificar si ya existe un código duplicado (excluyendo el actual)
                var existsByCode = await _scaleGroupRepository.ExistsByCodeAsync(requestDto.Code, id);
                if (existsByCode)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "Ya existe un grupo de escala con este código.", MessageType = ApplicationMessageType.Error });
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _scaleGroupRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<ScaleGroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<ScaleGroupResponseDto>(ex.Message);
            }
            return response;
        }
    }
}