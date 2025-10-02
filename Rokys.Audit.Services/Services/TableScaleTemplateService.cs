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

        public TableScaleTemplateService(
            ITableScaleTemplateRepository tableScaleTemplateRepository,
            IValidator<TableScaleTemplateRequestDto> fluentValidator,
            ILogger<TableScaleTemplateService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _tableScaleTemplateRepository = tableScaleTemplateRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<TableScaleTemplateResponseDto>> Create(TableScaleTemplateRequestDto requestDto)
        {
            var response = ResponseDto.Create<TableScaleTemplateResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                // Verificar si ya existe un código duplicado
                var existsByCode = await _tableScaleTemplateRepository.ExistsByCodeAsync(requestDto.Code);
                if (existsByCode)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "Ya existe una plantilla de escala con este código.", MessageType = ApplicationMessageType.Error });
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<TableScaleTemplate>(requestDto);
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

        public async Task<ResponseDto<PaginationResponseDto<TableScaleTemplateResponseDto>>> GetPaged(PaginationRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<TableScaleTemplateResponseDto>>();
            try
            {
                Expression<Func<TableScaleTemplate, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = x => x.IsActive && (x.Name.Contains(paginationRequestDto.Filter) || x.Code.Contains(paginationRequestDto.Filter));

                Func<IQueryable<TableScaleTemplate>, IOrderedQueryable<TableScaleTemplate>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _tableScaleTemplateRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize,
                    includeProperties: [a => a.ScaleGroup]
                );

                var pagedResult = new PaginationResponseDto<TableScaleTemplateResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<TableScaleTemplateResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
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
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                var entity = await _tableScaleTemplateRepository.GetFirstOrDefaultAsync(filter: x => x.TableScaleTemplateId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<TableScaleTemplateResponseDto>("No se encontró la plantilla de escala.");
                    return response;
                }

                // Verificar si ya existe un código duplicado (excluyendo el actual)
                var existsByCode = await _tableScaleTemplateRepository.ExistsByCodeAsync(requestDto.Code, id);
                if (existsByCode)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "Ya existe una plantilla de escala con este código.", MessageType = ApplicationMessageType.Error });
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
    }
}