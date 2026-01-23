using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditRoleConfiguration;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.AuditRoleConfiguration;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Services.Validations;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class AuditRoleConfigurationService : IAuditRoleConfigurationService
    {
        private readonly IAuditRoleConfigurationRepository _auditRoleConfigurationRepository;
        private readonly IValidator<AuditRoleConfigurationRequestDto> _fluentValidator;
        private readonly ILogger<AuditRoleConfigurationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditRoleConfigurationService(
            IAuditRoleConfigurationRepository auditRoleConfigurationRepository,
            IValidator<AuditRoleConfigurationRequestDto> fluentValidator,
            ILogger<AuditRoleConfigurationService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _auditRoleConfigurationRepository = auditRoleConfigurationRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<AuditRoleConfigurationResponseDto>> Create(AuditRoleConfigurationRequestDto requestDto)
        {
            var response = ResponseDto.Create<AuditRoleConfigurationResponseDto>();
            try
            {
                var validate = await _fluentValidator.ValidateAsync(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<AuditRoleConfiguration>(requestDto);
                var existingSortOrders = (await _auditRoleConfigurationRepository.GetAsync(filter: x => (
                    (requestDto.EnterpriseId == null && x.EnterpriseId == null) ||
                    (requestDto.EnterpriseId != null && x.EnterpriseId == requestDto.EnterpriseId)        
                ) && x.IsActive))
                    .Select(x => x.SequenceOrder ?? 0);
                entity.SequenceOrder = Rokys.Audit.Common.Helpers.SortOrderHelper.GetNextSortOrder(existingSortOrders);

                entity.CreateAudit(currentUser?.UserName ?? "system");
                
                _auditRoleConfigurationRepository.Insert(entity);
                await _unitOfWork.CommitAsync();

                var entityCreate = await _auditRoleConfigurationRepository.GetFirstOrDefaultAsync(filter: x => x.AuditRoleConfigurationId == entity.AuditRoleConfigurationId && x.IsActive, includeProperties: [t => t.Enterprise]);
                response.Data = _mapper.Map<AuditRoleConfigurationResponseDto>(entityCreate);
                _logger.LogInformation("Created audit role configuration with ID: {Id}", entity.AuditRoleConfigurationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit role configuration");
                response = ResponseDto.Error<AuditRoleConfigurationResponseDto>("Error al crear la configuración de rol de auditoría");
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _auditRoleConfigurationRepository.GetFirstOrDefaultAsync(filter: x => x.AuditRoleConfigurationId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró la configuración de rol de auditoría.");
                    return response;
                }
                
                entity.IsActive = false;
                var currentUser = _httpContextAccessor.CurrentUser();
                entity.UpdateAudit(currentUser?.UserName ?? "system");
                
                _auditRoleConfigurationRepository.Delete(entity);
                await _unitOfWork.CommitAsync();
                
                _logger.LogInformation("Deleted audit role configuration with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting audit role configuration with ID: {Id}", id);
                response = ResponseDto.Error("Error al eliminar la configuración de rol de auditoría");
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<AuditRoleConfigurationResponseDto>>> GetPaged(AuditRoleConfigurationFilterRequestDto requestDto)
				{
						var response = ResponseDto.Create<PaginationResponseDto<AuditRoleConfigurationResponseDto>>();
						try
						{
								Expression<Func<AuditRoleConfiguration, bool>> filter = x => x.IsActive;

								Func<IQueryable<AuditRoleConfiguration>, IOrderedQueryable<AuditRoleConfiguration>> orderBy = 
										q => q.OrderBy(x => x.SequenceOrder ?? int.MaxValue).ThenBy(x => x.RoleName);

								if (!string.IsNullOrEmpty(requestDto.Filter))
										filter = filter.AndAlso(x => x.RoleCode.Contains(requestDto.Filter) || x.RoleName.Contains(requestDto.Filter));

								// Lógica del filtro de EnterpriseId
								if (requestDto.EnterpriseId.HasValue)
								{
										if (requestDto.EnterpriseId.Value == Guid.Empty)
										{
												// Guid.Empty significa "filtrar solo los que tienen EnterpriseId null"
												filter = filter.AndAlso(x => x.EnterpriseId == null);
										}
										else
										{
												// Cualquier otro Guid: filtrar por ese valor específico
												filter = filter.AndAlso(x => x.EnterpriseId == requestDto.EnterpriseId.Value);
										}
								}

								var entities = await _auditRoleConfigurationRepository.GetPagedAsync(
										filter: filter,
										orderBy: orderBy,
										pageNumber: requestDto.PageNumber,
										pageSize: requestDto.PageSize,
										includeProperties: [e => e.Enterprise]
								);

								var pagedResult = new PaginationResponseDto<AuditRoleConfigurationResponseDto>
								{
										Items = _mapper.Map<IEnumerable<AuditRoleConfigurationResponseDto>>(entities.Items),
										TotalCount = entities.TotalRows,
										PageNumber = requestDto.PageNumber,
										PageSize = requestDto.PageSize
								};

								response.Data = pagedResult;
						}
						catch (Exception ex)
						{
								_logger.LogError(ex, "Error retrieving paged audit role configurations");
								response = ResponseDto.Error<PaginationResponseDto<AuditRoleConfigurationResponseDto>>("Error al obtener las configuraciones de roles de auditoría");
						}
						return response;
				}

        public async Task<ResponseDto<AuditRoleConfigurationResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<AuditRoleConfigurationResponseDto>();
            try
            {
                var entity = await _auditRoleConfigurationRepository.GetFirstOrDefaultAsync(
                    filter: x => x.AuditRoleConfigurationId == id && x.IsActive, includeProperties: [e => e.Enterprise]
                );
                
                if (entity == null)
                {
                    response = ResponseDto.Error<AuditRoleConfigurationResponseDto>("No se encontró la configuración de rol de auditoría.");
                    return response;
                }
                
                response.Data = _mapper.Map<AuditRoleConfigurationResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit role configuration with ID: {Id}", id);
                response = ResponseDto.Error<AuditRoleConfigurationResponseDto>("Error al obtener la configuración de rol de auditoría");
            }
            return response;
        }

        public async Task<ResponseDto<AuditRoleConfigurationResponseDto>> Update(Guid id, AuditRoleConfigurationRequestDto requestDto)
        {
            var response = ResponseDto.Create<AuditRoleConfigurationResponseDto>();
            try
            {
                var validator = new AuditRoleConfigurationValidator(_auditRoleConfigurationRepository, id);
                var validate = await validator.ValidateAsync(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage
                    {
                        Message = e.ErrorMessage,
                        MessageType = ApplicationMessageType.Error
                    }));
                    return response;
                }

                var entity = await _auditRoleConfigurationRepository.GetFirstOrDefaultAsync(filter: x => x.AuditRoleConfigurationId == id && x.IsActive);
                if (entity == null)
                    return ResponseDto.Error<AuditRoleConfigurationResponseDto>("No se encontró la configuración de rol de auditoría.");
                requestDto.SequenceOrder = entity.SequenceOrder;
                var currentUser = _httpContextAccessor.CurrentUser();
                _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser?.UserName ?? "system");
                
                _auditRoleConfigurationRepository.Update(entity);
                await _unitOfWork.CommitAsync();

                var entityUpdate = await _auditRoleConfigurationRepository.GetFirstOrDefaultAsync(filter: x => x.AuditRoleConfigurationId == id && x.IsActive, includeProperties: [e => e.Enterprise]);
                response.Data = _mapper.Map<AuditRoleConfigurationResponseDto>(entityUpdate);
                _logger.LogInformation("Updated audit role configuration with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating audit role configuration with ID: {Id}", id);
                response = ResponseDto.Error<AuditRoleConfigurationResponseDto>("Error al actualizar la configuración de rol de auditoría");
            }
            return response;
        }

        public async Task<ResponseDto<List<AuditRoleConfigurationResponseDto>>> GetActiveConfigurationsAsync()
        {
            var response = ResponseDto.Create<List<AuditRoleConfigurationResponseDto>>();
            
            try
            {
                var configurations = await _auditRoleConfigurationRepository.GetActiveConfigurationsOrderedAsync();
                response.Data = _mapper.Map<List<AuditRoleConfigurationResponseDto>>(configurations);
                
                _logger.LogInformation("Retrieved {Count} active audit role configurations", configurations.Count());
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active audit role configurations");
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error al obtener las configuraciones de roles de auditoría",
                    MessageType = ApplicationMessageType.Error
                });
                return response;
            }
        }

        public async Task<ResponseDto<bool>> ChangeOrder(int currentPosition, int newPosition, Guid? enterpriseId)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                var items = (await _auditRoleConfigurationRepository.GetAsync(filter: x => (
                    (enterpriseId == null && x.EnterpriseId == null) ||
                    (enterpriseId != null && x.EnterpriseId == enterpriseId)
                ) && x.IsActive))
                    .Where(x => x.SequenceOrder.HasValue)
                    .OrderBy(x => x.SequenceOrder)
                    .ToList();

                var currentIndex = items.FindIndex(x => x.SequenceOrder == currentPosition);
                var newIndex = items.FindIndex(x => x.SequenceOrder == newPosition);
                
                if (currentIndex < 0 || newIndex < 0)
                {
                    response = ResponseDto.Error<bool>("Orden de secuencia no encontrado.");
                    return response;
                }

                var item = items[currentIndex];
                items.RemoveAt(currentIndex);
                items.Insert(newIndex, item);

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SequenceOrder = i + 1;
                    _auditRoleConfigurationRepository.Update(items[i]);
                }
                
                await _unitOfWork.CommitAsync();
                response.Data = true;
                _logger.LogInformation("Changed order from position {CurrentPosition} to {NewPosition}", currentPosition, newPosition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing order from position {CurrentPosition} to {NewPosition}", currentPosition, newPosition);
                response = ResponseDto.Error<bool>("Error al cambiar el orden de la configuración");
            }
            return response;
        }
    }
}