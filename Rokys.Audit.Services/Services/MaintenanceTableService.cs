using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.MaintenanceTable;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.MaintenanceTable;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class MaintenanceTableService : IMaintenanceTableService
    {
        private readonly IMaintenanceTableRepository _maintenanceTableRepository;
        private readonly IValidator<MaintenanceTableRequestDto> _validator;
        private readonly ILogger<MaintenanceTableService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MaintenanceTableService(
            IMaintenanceTableRepository maintenanceTableRepository,
            IValidator<MaintenanceTableRequestDto> validator,
            ILogger<MaintenanceTableService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _maintenanceTableRepository = maintenanceTableRepository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<MaintenanceTableResponseDto>> Create(MaintenanceTableRequestDto requestDto)
        {
            var response = ResponseDto.Create<MaintenanceTableResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<MaintenanceTable>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                entity.IsActive = true;
                _maintenanceTableRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<MaintenanceTableResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<MaintenanceTableResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _maintenanceTableRepository.GetFirstOrDefaultAsync(filter: x => x.MaintenanceTableId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el registro.");
                    return response;
                }
                entity.IsActive = false;
                entity.UpdateDate = DateTime.UtcNow;
                _maintenanceTableRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<MaintenanceTableResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<MaintenanceTableResponseDto>();
            try
            {
                var entity = await _maintenanceTableRepository.GetFirstOrDefaultAsync(filter: x => x.MaintenanceTableId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<MaintenanceTableResponseDto>("No se encontró la tabla de mantenimiento.");
                    return response;
                }
                response.Data = _mapper.Map<MaintenanceTableResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<MaintenanceTableResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<MaintenanceTableResponseDto>>> GetPaged(MaintenanceTableFilterRequestDto requestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<MaintenanceTableResponseDto>>();
            try
            {
                int totalRows;
                Expression<Func<MaintenanceTable, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(requestDto.Filter))
                    filter = filter.AndAlso(x => x.Code.Contains(requestDto.Filter) || x.Description.Contains(requestDto.Filter));

                if(!string.IsNullOrEmpty(requestDto.Code))
                    filter = filter.AndAlso(x => x.Code == requestDto.Code && x.IsActive);

                Func<IQueryable<MaintenanceTable>, IOrderedQueryable<MaintenanceTable>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _maintenanceTableRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: requestDto.PageNumber,
                    pageSize: requestDto.PageSize
                );

                var pagedResult = new PaginationResponseDto<MaintenanceTableResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<MaintenanceTableResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = requestDto.PageNumber,
                    PageSize = requestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<MaintenanceTableResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<MaintenanceTableResponseDto>> Update(Guid id, MaintenanceTableRequestDto requestDto)
        {
            var response = ResponseDto.Create<MaintenanceTableResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _maintenanceTableRepository.GetFirstOrDefaultAsync(filter: x => x.MaintenanceTableId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<MaintenanceTableResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _maintenanceTableRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<MaintenanceTableResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<MaintenanceTableResponseDto>(ex.Message);
            }
            return response;
        }
    }
}
