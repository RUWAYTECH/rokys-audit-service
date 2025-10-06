using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditStatus;
using Rokys.Audit.DTOs.Responses.AuditStatus;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class AuditStatusService : IAuditStatusService
    {
        private readonly IAuditStatusRepository _repository;
        private readonly IValidator<AuditStatusRequestDto> _validator;
        private readonly ILogger<AuditStatusService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditStatusService(
            IAuditStatusRepository repository,
            IValidator<AuditStatusRequestDto> validator,
            ILogger<AuditStatusService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<AuditStatusResponseDto>> Create(AuditStatusRequestDto requestDto)
        {
            var response = ResponseDto.Create<AuditStatusResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<AuditStatus>(requestDto);
                entity.CreatedBy = currentUser.UserName;
                entity.CreationDate = DateTime.UtcNow;
                entity.IsActive = true;
                _repository.Insert(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<AuditStatusResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<AuditStatusResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.AuditStatusId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el registro.");
                    return response;
                }
                entity.IsActive = false;
                entity.UpdateDate = DateTime.UtcNow;
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<AuditStatusResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<AuditStatusResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.AuditStatusId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<AuditStatusResponseDto>("No se encontró el registro.");
                    return response;
                }
                response.Data = _mapper.Map<AuditStatusResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<AuditStatusResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<AuditStatusResponseDto>> Update(Guid id, AuditStatusRequestDto requestDto)
        {
            var response = ResponseDto.Create<AuditStatusResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.AuditStatusId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<AuditStatusResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdatedBy = currentUser.UserName;
                entity.UpdateDate = DateTime.UtcNow;
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<AuditStatusResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<AuditStatusResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<AuditStatusResponseDto>>> GetPaged(PaginationRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<AuditStatusResponseDto>>();
            try
            {
                Expression<Func<AuditStatus, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = x => x.Name.Contains(paginationRequestDto.Filter) && x.IsActive;

                Func<IQueryable<AuditStatus>, IOrderedQueryable<AuditStatus>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _repository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize
                );

                var pagedResult = new PaginationResponseDto<AuditStatusResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<AuditStatusResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<AuditStatusResponseDto>>(ex.Message);
            }
            return response;
        }
    }
}
