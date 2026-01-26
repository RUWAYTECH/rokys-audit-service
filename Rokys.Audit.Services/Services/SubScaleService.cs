using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.SubScale;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.SubScale;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class SubScaleService : ISubScaleService
    {
        private readonly ISubScaleRepository _subScaleRepository;
        private readonly IValidator<SubScaleRequestDto> _fluentValidator;
        private readonly ILogger<SubScaleService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubScaleService(
            ISubScaleRepository subScaleRepository,
            IValidator<SubScaleRequestDto> fluentValidator,
            ILogger<SubScaleService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _subScaleRepository = subScaleRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<SubScaleResponseDto>> Create(SubScaleRequestDto requestDto)
        {
            var response = ResponseDto.Create<SubScaleResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                
                var entity = _mapper.Map<SubScale>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                _subScaleRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                
                var entityCreate = await _subScaleRepository.GetFirstOrDefaultAsync(
                    filter: x => x.SubScaleId == entity.SubScaleId && x.IsActive, 
                    includeProperties: [t => t.ScaleCompany]);
                response.Data = _mapper.Map<SubScaleResponseDto>(entityCreate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<SubScaleResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _subScaleRepository.GetFirstOrDefaultAsync(filter: x => x.SubScaleId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró la sub escala.");
                    return response;
                }
                entity.IsActive = false;
                _subScaleRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<SubScaleResponseDto>>> GetPaged(SubScaleFilterRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<SubScaleResponseDto>>();
            try
            {
                Expression<Func<SubScale, bool>> filter = x => x.IsActive;
                
                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = filter.AndAlso(x => x.Name.Contains(paginationRequestDto.Filter) || x.Code.Contains(paginationRequestDto.Filter));

                if (paginationRequestDto.ScaleCompanyId.HasValue)
                {
                    filter = filter.AndAlso(x => x.ScaleCompanyId == paginationRequestDto.ScaleCompanyId.Value);
                }

                Func<IQueryable<SubScale>, IOrderedQueryable<SubScale>> orderBy = q => q.OrderBy(x => x.ScaleCompanyId).ThenBy(x => x.Code);

                var entities = await _subScaleRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize,
                    includeProperties: [e => e.ScaleCompany]
                );

                var pagedResult = new PaginationResponseDto<SubScaleResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<SubScaleResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<SubScaleResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<SubScaleResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<SubScaleResponseDto>();
            try
            {
                var entity = await _subScaleRepository.GetFirstOrDefaultAsync(
                    filter: x => x.SubScaleId == id && x.IsActive, 
                    includeProperties: [e => e.ScaleCompany]);
                if (entity == null)
                {
                    response = ResponseDto.Error<SubScaleResponseDto>("No se encontró la sub escala.");
                    return response;
                }
                response.Data = _mapper.Map<SubScaleResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<SubScaleResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<SubScaleResponseDto>> Update(Guid id, SubScaleRequestDto requestDto)
        {
            var response = ResponseDto.Create<SubScaleResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _subScaleRepository.GetFirstOrDefaultAsync(filter: x => x.SubScaleId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<SubScaleResponseDto>("No se encontró la sub escala.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _subScaleRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                
                var entityUpdate = await _subScaleRepository.GetFirstOrDefaultAsync(
                    filter: x => x.SubScaleId == id && x.IsActive, 
                    includeProperties: [e => e.ScaleCompany]);
                response.Data = _mapper.Map<SubScaleResponseDto>(entityUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<SubScaleResponseDto>(ex.Message);
            }
            return response;
        }
    }
}
