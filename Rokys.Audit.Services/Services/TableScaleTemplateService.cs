using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.TableScaleTemplate;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.TableScaleTemplate;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Services.Validations;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class TableScaleTemplateService : ITableScaleTemplateService
    {
        private readonly ITableScaleTemplateRepository _tableScaleTemplateRepository;
        private readonly IValidator<TableScaleTemplateRequestDto> _fluentValidator;
        private readonly ILogger<TableScaleTemplateService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditTemplateFieldRepository _auditTemplateFieldRepository;

        public TableScaleTemplateService(
            ITableScaleTemplateRepository tableScaleTemplateRepository,
            IValidator<TableScaleTemplateRequestDto> fluentValidator,
            ILogger<TableScaleTemplateService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IAuditTemplateFieldRepository auditTemplateFieldRepository)
        {
            _tableScaleTemplateRepository = tableScaleTemplateRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _auditTemplateFieldRepository = auditTemplateFieldRepository;
        }

        public async Task<ResponseDto<TableScaleTemplateResponseDto>> Create(TableScaleTemplateRequestDto requestDto)
        {
            var response = ResponseDto.Create<TableScaleTemplateResponseDto>();
            try
            {
                var validate = await _fluentValidator.ValidateAsync(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                if (string.IsNullOrEmpty(requestDto.TemplateData))
                {
                    requestDto.TemplateData = "[]";
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<TableScaleTemplate>(requestDto);

                // Ensure SortOrder is assigned server-side: next value within the ScaleGroup
                var existingSortOrders = (await _tableScaleTemplateRepository.GetAsync(filter: x => x.ScaleGroupId == requestDto.ScaleGroupId && x.IsActive))
                    .Select(x => x.SortOrder);
                entity.SortOrder = Rokys.Audit.Common.Helpers.SortOrderHelper.GetNextSortOrder(existingSortOrders);

                entity.CreateAudit(currentUser.UserName);
                _tableScaleTemplateRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                var getCreatedData = await _tableScaleTemplateRepository.GetFirstOrDefaultAsync(
                    filter: x => x.TableScaleTemplateId == entity.TableScaleTemplateId && x.IsActive, includeProperties: [g => g.ScaleGroup]);
                response.Data = _mapper.Map<TableScaleTemplateResponseDto>(getCreatedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<TableScaleTemplateResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var hasDependentFields = await _auditTemplateFieldRepository.GetFirstOrDefaultAsync(filter: x=>x.TableScaleTemplateId == id && x.IsActive);
                if (hasDependentFields != null)
                {
                    response = ResponseDto.Error("No se puede eliminar la plantilla de escala porque tiene campos de plantilla de auditoría asociados.");
                    return response;
                }
                var entity = await _tableScaleTemplateRepository.GetFirstOrDefaultAsync(filter: x => x.TableScaleTemplateId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró la plantilla de escala.");
                    return response;
                }
                entity.IsActive = false;
                var currentUser = _httpContextAccessor.CurrentUser();
                entity.UpdateAudit(currentUser.UserName);
                _tableScaleTemplateRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<TableScaleTemplateResponseDto>>> GetPaged(TableScaleTemplateFilterRequestDto filterRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<TableScaleTemplateResponseDto>>();
            try
            {
                var filter = BuildFilter(filterRequestDto);

                Func<IQueryable<TableScaleTemplate>, IOrderedQueryable<TableScaleTemplate>> orderBy = q => q.OrderBy(x => x.SortOrder);
                var entities = await _tableScaleTemplateRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: filterRequestDto.PageNumber,
                    pageSize: filterRequestDto.PageSize,
                    includeProperties: [a => a.ScaleGroup]
                );

                var pagedResult = new PaginationResponseDto<TableScaleTemplateResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<TableScaleTemplateResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = filterRequestDto.PageNumber,
                    PageSize = filterRequestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<TableScaleTemplateResponseDto>>(ex.Message);
            }
            return response;
        }

        private Expression<Func<TableScaleTemplate, bool>> BuildFilter(TableScaleTemplateFilterRequestDto dto)
        {
            return x =>
                x.IsActive &&
                (!dto.ScaleGroupId.HasValue || x.ScaleGroupId == dto.ScaleGroupId.Value) &&
                (string.IsNullOrEmpty(dto.Filter) || x.Name.Contains(dto.Filter));
        }

        public async Task<ResponseDto<TableScaleTemplateResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<TableScaleTemplateResponseDto>();
            try
            {
                var entity = await _tableScaleTemplateRepository.GetFirstOrDefaultAsync(
                    filter: x => x.TableScaleTemplateId == id && x.IsActive,
                    includeProperties: [a => a.ScaleGroup]
                );
                if (entity == null)
                {
                    response = ResponseDto.Error<TableScaleTemplateResponseDto>("No se encontró la plantilla de escala.");
                    return response;
                }
                response.Data = _mapper.Map<TableScaleTemplateResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<TableScaleTemplateResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<IEnumerable<TableScaleTemplateResponseDto>>> GetByScaleGroupId(Guid scaleGroupId)
        {
            var response = ResponseDto.Create<IEnumerable<TableScaleTemplateResponseDto>>();
            try
            {
                var entities = await _tableScaleTemplateRepository.GetByScaleGroupIdAsync(scaleGroupId);
                response.Data = _mapper.Map<IEnumerable<TableScaleTemplateResponseDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<IEnumerable<TableScaleTemplateResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<TableScaleTemplateResponseDto>> Update(Guid id, TableScaleTemplateRequestDto requestDto)
        {
            var response = ResponseDto.Create<TableScaleTemplateResponseDto>();
            try
            {
                var validator = new TableScaleTemplateValidator(_tableScaleTemplateRepository, id);
                var validate = await validator.ValidateAsync(requestDto);

                var entity = await _tableScaleTemplateRepository.GetFirstOrDefaultAsync(filter: x => x.TableScaleTemplateId == id && x.IsActive, includeProperties: [x=>x.ScaleGroup]);
                if (entity == null)
                {
                    response = ResponseDto.Error<TableScaleTemplateResponseDto>("No se encontró la plantilla de escala.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _tableScaleTemplateRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<TableScaleTemplateResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<TableScaleTemplateResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<bool>> ChangeOrder(Guid scaleGroupId, int currentPosition, int newPosition)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                // get all templates for group ordered by SortOrder
                var items = (await _tableScaleTemplateRepository.GetAsync(filter: x => x.ScaleGroupId == scaleGroupId && x.IsActive))
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                var currentIndex = items.FindIndex(x => x.SortOrder == currentPosition);
                var newIndex = items.FindIndex(x => x.SortOrder == newPosition);
                if (currentIndex < 0 || newIndex < 0)
                {
                    response = ResponseDto.Error<bool>("SortOrder no encontrado en el grupo.");
                    return response;
                }

                var item = items[currentIndex];
                items.RemoveAt(currentIndex);
                items.Insert(newIndex, item);

                // update sort orders sequentially starting at 1
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SortOrder = i + 1;
                    _tableScaleTemplateRepository.Update(items[i]);
                }
                await _unitOfWork.CommitAsync();
                response.Data = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<bool>(ex.Message);
            }
            return response;
        }
    }
}