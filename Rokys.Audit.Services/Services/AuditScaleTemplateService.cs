using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditScaleTemplate;
using Rokys.Audit.DTOs.Responses.AuditScaleTemplate;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class AuditScaleTemplateService : IAuditScaleTemplateService
    {
        private readonly IAuditScaleTemplateRepository _auditScaleTemplateRepository;
        private readonly IValidator<AuditScaleTemplateRequestDto> _fluentValidator;
        private readonly ILogger<AuditScaleTemplateService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditScaleTemplateService(
            IAuditScaleTemplateRepository auditScaleTemplateRepository,
            IValidator<AuditScaleTemplateRequestDto> fluentValidator,
            ILogger<AuditScaleTemplateService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _auditScaleTemplateRepository = auditScaleTemplateRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<AuditScaleTemplateResponseDto>> Create(AuditScaleTemplateRequestDto requestDto)
        {
            var response = ResponseDto.Create<AuditScaleTemplateResponseDto>();
            try
            {
                var validate = await _fluentValidator.ValidateAsync(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<AuditScaleTemplate>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                _auditScaleTemplateRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<AuditScaleTemplateResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<AuditScaleTemplateResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _auditScaleTemplateRepository.GetFirstOrDefaultAsync(filter: x => x.AuditScaleTemplateId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró la plantilla de escala de auditoría.");
                    return response;
                }
                entity.IsActive = false;
                _auditScaleTemplateRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<AuditScaleTemplateResponseDto>>> GetPaged(PaginationRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<AuditScaleTemplateResponseDto>>();
            try
            {
                Expression<Func<AuditScaleTemplate, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = x => x.Name.Contains(paginationRequestDto.Filter) || x.Code.Contains(paginationRequestDto.Filter);

                Func<IQueryable<AuditScaleTemplate>, IOrderedQueryable<AuditScaleTemplate>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _auditScaleTemplateRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize
                );

                var pagedResult = new PaginationResponseDto<AuditScaleTemplateResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<AuditScaleTemplateResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<AuditScaleTemplateResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<AuditScaleTemplateResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<AuditScaleTemplateResponseDto>();
            try
            {
                var entity = await _auditScaleTemplateRepository.GetFirstOrDefaultAsync(filter: x => x.AuditScaleTemplateId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<AuditScaleTemplateResponseDto>("No se encontró la plantilla de escala de auditoría.");
                    return response;
                }
                response.Data = _mapper.Map<AuditScaleTemplateResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<AuditScaleTemplateResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<AuditScaleTemplateResponseDto>> Update(Guid id, AuditScaleTemplateRequestDto requestDto)
        {
            var response = ResponseDto.Create<AuditScaleTemplateResponseDto>();
            try
            {
                var validate = await _fluentValidator.ValidateAsync(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                var entity = await _auditScaleTemplateRepository.GetFirstOrDefaultAsync(filter: x => x.AuditScaleTemplateId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<AuditScaleTemplateResponseDto>("No se encontró la plantilla de escala de auditoría.");
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _auditScaleTemplateRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<AuditScaleTemplateResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<AuditScaleTemplateResponseDto>(ex.Message);
            }
            return response;
        }
    }
}