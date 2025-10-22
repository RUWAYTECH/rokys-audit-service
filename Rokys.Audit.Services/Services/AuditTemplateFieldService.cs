using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditTemplateField;
using Rokys.Audit.DTOs.Responses.AuditTemplateField;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Enterprise;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Services.Validations;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class AuditTemplateFieldService : IAuditTemplateFieldService
    {
    private readonly IAuditTemplateFieldRepository _auditTemplateFieldRepository;
    private readonly ITableScaleTemplateRepository _tableScaleTemplateRepository;
        private readonly IValidator<AuditTemplateFieldRequestDto> _fluentValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditTemplateFieldService(
            IAuditTemplateFieldRepository auditTemplateFieldRepository,
            ITableScaleTemplateRepository tableScaleTemplateRepository,
            IValidator<AuditTemplateFieldRequestDto> fluentValidator,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            ILogger<AuditTemplateFieldService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _auditTemplateFieldRepository = auditTemplateFieldRepository;
            _tableScaleTemplateRepository = tableScaleTemplateRepository;
            _fluentValidator = fluentValidator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<AuditTemplateFieldResponseDto>> Create(AuditTemplateFieldRequestDto requestDto)
        {
            var response = ResponseDto.Create<AuditTemplateFieldResponseDto>();
            try
            {
                var validationResult = await _fluentValidator.ValidateAsync(requestDto);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        response.Messages.Add(new ApplicationMessage
                        {
                            Message = error.ErrorMessage,
                            MessageType = ApplicationMessageType.Error
                        });
                    }
                    return response;
                }

                if (string.IsNullOrEmpty(requestDto.FieldOptions))
                {
                    requestDto.FieldOptions = "[]";
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                var userName = currentUser?.UserName ?? "system";
                var entity = _mapper.Map<AuditTemplateFields>(requestDto);

                // Assign SortOrder server-side: next value within the TableScaleTemplateId group
                var existingSortOrders = (await _auditTemplateFieldRepository.GetAsync(filter: x => x.TableScaleTemplateId == requestDto.TableScaleTemplateId && x.IsActive))
                    .Select(x => x.SortOrder);
                entity.SortOrder = Rokys.Audit.Common.Helpers.SortOrderHelper.GetNextSortOrder(existingSortOrders);

                entity.CreateAudit(userName);
                _auditTemplateFieldRepository.Insert(entity);
                await _unitOfWork.CommitAsync();

                // Map the in-memory entity to response DTO and enrich with TableScaleTemplate info
                var createdEntity = await _auditTemplateFieldRepository.GetFirstOrDefaultAsync(filter: x => x.AuditTemplateFieldId == entity.AuditTemplateFieldId && x.IsActive,
                    includeProperties: [at => at.TableScaleTemplate]);
                
                response.Data = _mapper.Map<AuditTemplateFieldResponseDto>(createdEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<AuditTemplateFieldResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<bool>> ChangeOrder(AuditTemplateFieldOrderRequestDto request)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                var items = (await _auditTemplateFieldRepository.GetAsync(filter: x => x.TableScaleTemplateId == request.TableScaleTemplateId && x.IsActive))
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                var currentIndex = items.FindIndex(x => x.SortOrder == request.CurrentPosition);
                var newIndex = items.FindIndex(x => x.SortOrder == request.NewPosition);
                if (currentIndex < 0 || newIndex < 0)
                {
                    response = ResponseDto.Error<bool>("SortOrder no encontrado en la plantilla.");
                    return response;
                }

                var item = items[currentIndex];
                items.RemoveAt(currentIndex);
                items.Insert(newIndex, item);

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SortOrder = i + 1;
                    _auditTemplateFieldRepository.Update(items[i]);
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

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _auditTemplateFieldRepository.GetFirstOrDefaultAsync(filter: x => x.AuditTemplateFieldId == id && x.IsActive, includeProperties: [at => at.TableScaleTemplate]);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontro el template.");
                    return response;
                }
                entity.IsActive = false;
                _auditTemplateFieldRepository.Delete(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<AuditTemplateFieldResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<AuditTemplateFieldResponseDto>();
            try
            {
                var entity = await _auditTemplateFieldRepository.GetFirstOrDefaultAsync(filter: x => x.AuditTemplateFieldId == id && x.IsActive, includeProperties: [at => at.TableScaleTemplate]);
                if (entity == null)
                {
                    response = ResponseDto.Error<AuditTemplateFieldResponseDto>("No se encontro el template.");
                    return response;
                }
                var responseData = _mapper.Map<AuditTemplateFieldResponseDto>(entity);
                response = ResponseDto.Create(responseData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<AuditTemplateFieldResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<AuditTemplateFieldResponseDto>>> GetPaged(AuditTemplateFieldFilterRequestDto requestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<AuditTemplateFieldResponseDto>>();
            try
            {
                Expression<Func<AuditTemplateFields, bool>> filter = x => x.IsActive;

                if (!string.IsNullOrEmpty(requestDto.Filter))
                    filter = filter.AndAlso(x => x.FieldName.Contains(requestDto.Filter));

                if (requestDto.TableScaleTemplateId.HasValue)
                    filter = filter.AndAlso(x => x.TableScaleTemplateId == requestDto.TableScaleTemplateId.Value);

                if (requestDto.ScaleGroupId.HasValue)
                    filter = filter.AndAlso(x => x.TableScaleTemplate.ScaleGroupId == requestDto.ScaleGroupId.Value);

                Func<IQueryable<AuditTemplateFields>, IOrderedQueryable<AuditTemplateFields>> orderBy = q => q.OrderBy(x => x.SortOrder);

                var entities = await _auditTemplateFieldRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: requestDto.PageNumber,
                    pageSize: requestDto.PageSize,
                    includeProperties: [at => at.TableScaleTemplate]
                );

                var pagedResult = new PaginationResponseDto<AuditTemplateFieldResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<AuditTemplateFieldResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = requestDto.PageNumber,
                    PageSize = requestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<PaginationResponseDto<AuditTemplateFieldResponseDto>>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<AuditTemplateFieldResponseDto>> Update(Guid id, AuditTemplateFieldRequestDto requestDto)
        {
            var response = ResponseDto.Create<AuditTemplateFieldResponseDto>();
            try
            {
                var validator = new AuditTemplateFieldValidator(_auditTemplateFieldRepository, id);
                var validationResult = await validator.ValidateAsync(requestDto);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        response.Messages.Add(new ApplicationMessage
                        {
                            Message = error.ErrorMessage,
                            MessageType = ApplicationMessageType.Error
                        });
                    }
                    return response;
                }
                var entity = await _auditTemplateFieldRepository.GetFirstOrDefaultAsync(filter: x => x.AuditTemplateFieldId == id && x.IsActive, includeProperties: [x => x.TableScaleTemplate]);
                if (entity == null)
                {
                    response = ResponseDto.Error<AuditTemplateFieldResponseDto>("No se encontro el template.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var userName = currentUser?.UserName ?? "system";
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(userName);
                _auditTemplateFieldRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                var responseData = _mapper.Map<AuditTemplateFieldResponseDto>(entity);
                response = ResponseDto.Create(responseData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<AuditTemplateFieldResponseDto>(ex.Message);
            }
            return response;
        }
    }
}
