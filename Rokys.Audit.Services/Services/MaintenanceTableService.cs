using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<MaintenanceTableService> _logger;
        private readonly IAMapper _mapper;

        public MaintenanceTableService(
            IMaintenanceTableRepository maintenanceTableRepository,
            ILogger<MaintenanceTableService> logger,
            IAMapper mapper)
        {
            _maintenanceTableRepository = maintenanceTableRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public Task<ResponseDto<MaintenanceTableResponseDto>> Create(MaintenanceTableRequestDto requestDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> Delete(Guid id)
        {
            throw new NotImplementedException();
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
                    filter = x => x.Code.Contains(requestDto.Filter) || x.Description.Contains(requestDto.Filter);

                if(!string.IsNullOrEmpty(requestDto.Code))
                    filter = x => x.Code == requestDto.Code && x.IsActive;

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

        public Task<ResponseDto<MaintenanceTableResponseDto>> Update(Guid id, MaintenanceTableRequestDto requestDto)
        {
            throw new NotImplementedException();
        }
    }
}
