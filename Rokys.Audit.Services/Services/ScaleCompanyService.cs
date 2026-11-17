using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScaleCompany;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.ScaleCompany;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class ScaleCompanyService : IScaleCompanyService
    {
        private readonly IScaleCompanyRepository _scaleCompanyRepository;
        private readonly IValidator<ScaleCompanyRequestDto> _fluentValidator;
        private readonly ILogger<ScaleCompanyService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScaleCompanyService(
            IScaleCompanyRepository scaleCompanyRepository,
            IValidator<ScaleCompanyRequestDto> fluentValidator,
            ILogger<ScaleCompanyService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _scaleCompanyRepository = scaleCompanyRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<ScaleCompanyResponseDto>> Create(ScaleCompanyRequestDto requestDto)
        {
            var response = ResponseDto.Create<ScaleCompanyResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                // Obtener el último código existente
                var lastCode = _scaleCompanyRepository.Get(x => x.IsActive)
                    .OrderByDescending(x => x.CreationDate)
                    .Select(x => x.Code)
                    .FirstOrDefault();
                var nextCode = Rokys.Audit.Common.Helpers.CodeGeneratorHelper.GenerateNextCode("SC", lastCode, 4);
                var entity = _mapper.Map<ScaleCompany>(requestDto);
                entity.Code = nextCode;
                // Obtener el siguiente sortOrder para la empresa
                var existingSortOrders = _scaleCompanyRepository
                    .Get(x => x.EnterpriseId == entity.EnterpriseId)
                    .Select(x => x.SortOrder);
                entity.SortOrder = Rokys.Audit.Common.Helpers.SortOrderHelper.GetNextSortOrder(existingSortOrders);
                entity.CreateAudit(currentUser.UserName);
                _scaleCompanyRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                var entityCreate = await _scaleCompanyRepository.GetFirstOrDefaultAsync(filter: x => x.ScaleCompanyId == entity.ScaleCompanyId && x.IsActive, includeProperties: [t => t.Enterprise]);
                response.Data = _mapper.Map<ScaleCompanyResponseDto>(entityCreate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Contains("UNIQUE"))
                {
                    response = ResponseDto.Error<ScaleCompanyResponseDto>("No se pudo generar un código único. Intente nuevamente.");
                }
                else
                {
                    response = ResponseDto.Error<ScaleCompanyResponseDto>(ex.Message);
                }
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _scaleCompanyRepository.GetFirstOrDefaultAsync(filter: x=>x.ScaleCompanyId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontro la escala.");
                    return response;
                }
                entity.IsActive = false;
                _scaleCompanyRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<ScaleCompanyResponseDto>>> GetPaged(ScaleCompanyFilterRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<ScaleCompanyResponseDto>>();
            try
            {
                Expression<Func<ScaleCompany, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = filter.AndAlso(x => x.Name.Contains(paginationRequestDto.Filter));

                if (paginationRequestDto.EnterpriseGroupingId.HasValue)
                {
                    filter = filter.AndAlso(x => x.EnterpriseGroupingId == paginationRequestDto.EnterpriseGroupingId.Value);
                }

                if (paginationRequestDto.EnterpriseId.HasValue)
                {
                    filter = filter.AndAlso(x => x.EnterpriseId == paginationRequestDto.EnterpriseId.Value);
                }

                var entities = await _scaleCompanyRepository.GetCustomPagedAsync(
                    filter: filter,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize
                );

                var pagedResult = new PaginationResponseDto<ScaleCompanyResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<ScaleCompanyResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<ScaleCompanyResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<ScaleCompanyResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<ScaleCompanyResponseDto>();
            try
            {
                var entity = await _scaleCompanyRepository.GetFirstOrDefaultAsync(filter: x => x.ScaleCompanyId == id && x.IsActive, includeProperties: [ e => e.Enterprise, e => e.EnterpriseGrouping]);
                if (entity == null)
                {
                    response = ResponseDto.Error<ScaleCompanyResponseDto>("No se encontro la escala.");
                    return response;
                }
                response.Data = _mapper.Map<ScaleCompanyResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<ScaleCompanyResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<ScaleCompanyResponseDto>> Update(Guid id, ScaleCompanyRequestDto requestDto)
        {
            var response = ResponseDto.Create<ScaleCompanyResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _scaleCompanyRepository.GetFirstOrDefaultAsync(filter: x =>x .ScaleCompanyId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<ScaleCompanyResponseDto>("No se encontro la escala.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _scaleCompanyRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                var entityUpdate = await _scaleCompanyRepository.GetFirstOrDefaultAsync(filter: x => x.ScaleCompanyId == id && x.IsActive, includeProperties: [e => e.Enterprise]);
                response.Data = _mapper.Map<ScaleCompanyResponseDto>(entityUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<ScaleCompanyResponseDto>(ex.Message);
            }
            return response;
        }


    }
}
