using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.MaintenanceDetailTable;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.MaintenanceDetailTable;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rokys.Audit.Services.Services
{
    public class MaintenanceDetailTableService : IMaintenanceDetailTableService
    {
        private readonly IRepository<MaintenanceDetailTable> _repository;
        private readonly IValidator<MaintenanceDetailTableRequestDto> _validator;
        private readonly ILogger<MaintenanceDetailTableService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MaintenanceDetailTableService(
            IRepository<MaintenanceDetailTable> repository,
            IValidator<MaintenanceDetailTableRequestDto> validator,
            ILogger<MaintenanceDetailTableService> logger,
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

        public async Task<ResponseDto<MaintenanceDetailTableResponseDto>> Create(MaintenanceDetailTableRequestDto requestDto)
        {
            var response = ResponseDto.Create<MaintenanceDetailTableResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<MaintenanceDetailTable>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                entity.IsActive = true;
                _repository.Insert(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<MaintenanceDetailTableResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<MaintenanceDetailTableResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.MaintenanceDetailTableId == id && x.IsActive);
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

        public async Task<ResponseDto<MaintenanceDetailTableResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<MaintenanceDetailTableResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.MaintenanceDetailTableId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<MaintenanceDetailTableResponseDto>("No se encontró el registro.");
                    return response;
                }
                response.Data = _mapper.Map<MaintenanceDetailTableResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<MaintenanceDetailTableResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<MaintenanceDetailTableResponseDto>> Update(Guid id, MaintenanceDetailTableRequestDto requestDto)
        {
            var response = ResponseDto.Create<MaintenanceDetailTableResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.MaintenanceDetailTableId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<MaintenanceDetailTableResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<MaintenanceDetailTableResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<MaintenanceDetailTableResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<MaintenanceDetailTableResponseDto>>> GetPaged(MaintenanceDetailTableFilterRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<MaintenanceDetailTableResponseDto>>();
            try
            {
                Expression<Func<MaintenanceDetailTable, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = x => x.Code.Contains(paginationRequestDto.Filter) && x.IsActive;

                if (!string.IsNullOrEmpty(paginationRequestDto.MaintenanceTableCode))
                    filter = x => x.MaintenanceTable.Code.Contains(paginationRequestDto.MaintenanceTableCode) && x.MaintenanceTable.IsActive;

                Func<IQueryable<MaintenanceDetailTable>, IOrderedQueryable<MaintenanceDetailTable>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _repository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize
                );

                var pagedResult = new PaginationResponseDto<MaintenanceDetailTableResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<MaintenanceDetailTableResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<MaintenanceDetailTableResponseDto>>(ex.Message);
            }
            return response;
        }
    }
}
