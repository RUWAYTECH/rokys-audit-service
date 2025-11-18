using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.SubstitutionHistory;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.SubstitutionHistory;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class SubstitutionHistoryService : ISubstitutionHistoryService
    {
        private readonly ISubstitutionHistoryRepository _substitutionHistoryRepository;
        private readonly IPeriodAuditRepository _periodAuditRepository;
        private readonly IUserReferenceRepository _userReferenceRepository;
        private readonly IPeriodAuditParticipantRepository _periodAuditParticipantRepository;
        private readonly IValidator<SubstitutionHistoryRequestDto> _fluentValidator;
        private readonly ILogger<SubstitutionHistoryService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubstitutionHistoryService(
            ISubstitutionHistoryRepository substitutionHistoryRepository,
            IPeriodAuditRepository periodAuditRepository,
            IUserReferenceRepository userReferenceRepository,
            IPeriodAuditParticipantRepository periodAuditParticipantRepository,
            IValidator<SubstitutionHistoryRequestDto> fluentValidator,
            ILogger<SubstitutionHistoryService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _substitutionHistoryRepository = substitutionHistoryRepository;
            _periodAuditRepository = periodAuditRepository;
            _userReferenceRepository = userReferenceRepository;
            _periodAuditParticipantRepository = periodAuditParticipantRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<SubstitutionHistoryResponseDto>> Create(SubstitutionHistoryRequestDto requestDto)
        {
            var response = ResponseDto.Create<SubstitutionHistoryResponseDto>();
            try
            {
                // Validación de FluentValidation
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage
                    {
                        Message = e.ErrorMessage,
                        MessageType = ApplicationMessageType.Error
                    }));
                    return response;
                }

                // Validar que la auditoría exista y esté activa
                var periodAudit = await _periodAuditRepository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditId == requestDto.PeriodAuditId && x.IsActive,
                    includeProperties: [x => x.AuditStatus, x => x.Store, x => x.Store.Enterprise]
                );

                if (periodAudit == null)
                {
                    response = ResponseDto.Error<SubstitutionHistoryResponseDto>("La auditoría especificada no existe o no está activa.");
                    return response;
                }

                // Validar que la auditoría esté en estado "En proceso"
                if (periodAudit.AuditStatus?.Code != "PRO")
                {
                    response = ResponseDto.Error<SubstitutionHistoryResponseDto>(
                        $"Las suplencias solo se pueden registrar cuando la auditoría está en estado 'En proceso'. Estado actual: {periodAudit.AuditStatus?.Name ?? "Desconocido"}");
                    return response;
                }

                // Validar que el nuevo usuario exista y esté activo
                var newUser = await _userReferenceRepository.GetFirstOrDefaultAsync(
                    filter: x => x.UserReferenceId == requestDto.NewUserReferenceId && x.IsActive
                );

                if (newUser == null)
                {
                    response = ResponseDto.Error<SubstitutionHistoryResponseDto>("El nuevo usuario especificado no existe o no está activo.");
                    return response;
                }

                // Validar que el usuario anterior sea participante de la auditoría
                var participant = await _periodAuditParticipantRepository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditId == requestDto.PeriodAuditId
                              && x.UserReferenceId == requestDto.PreviousUserReferenceId
                              && x.IsActive
                );

                if (participant == null)
                {
                    response = ResponseDto.Error<SubstitutionHistoryResponseDto>(
                        "El usuario anterior especificado no es participante activo de esta auditoría.");
                    return response;
                }

                // Validar que el nuevo usuario tenga el mismo rol que el usuario anterior tenía en la auditoría
                // Usamos el RoleCodeSnapshot porque el usuario pudo haber cambiado de rol después de asignarse a la auditoría
                if (participant.RoleCodeSnapshot != newUser.RoleCode)
                {
                    response = ResponseDto.Error<SubstitutionHistoryResponseDto>(
                        $"El nuevo usuario debe tener el mismo rol que el usuario anterior en esta auditoría. Rol requerido: {participant.RoleNameSnapshot}, Rol del nuevo usuario: {newUser.RoleName}");
                    return response;
                }

                // Actualizar el participante con el nuevo usuario
                participant.UserReferenceId = newUser.UserReferenceId;
                // Mantenemos el mismo rol snapshot (ya que validamos que el nuevo usuario tiene ese rol)
                participant.RoleCodeSnapshot = newUser.RoleCode ?? string.Empty;
                participant.RoleNameSnapshot = newUser.RoleName ?? string.Empty;
                participant.UpdateAudit(_httpContextAccessor.CurrentUser()?.UserName);
                _periodAuditParticipantRepository.Update(participant);

                // Crear la entidad de suplencia
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<SubstitutionHistory>(requestDto);

                // Obtener el RoleName del nuevo usuario automáticamente
                entity.RoleName = newUser.RoleName ?? string.Empty;

                entity.CreateAudit(currentUser?.UserName);

                _substitutionHistoryRepository.Insert(entity);
                await _unitOfWork.CommitAsync();

                // Obtener la entidad creada con todas las relaciones
                var createdEntity = await _substitutionHistoryRepository.GetFirstOrDefaultAsync(
                    filter: x => x.SubstitutionHistoryId == entity.SubstitutionHistoryId,
                    includeProperties: [
                        x => x.PeriodAudit,
                        x => x.PeriodAudit.Store,
                        x => x.PeriodAudit.Store.Enterprise,
                        x => x.PreviousUserReference,
                        x => x.NewUserReference
                    ]
                );

                response.Data = _mapper.Map<SubstitutionHistoryResponseDto>(createdEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la suplencia");
                response = ResponseDto.Error<SubstitutionHistoryResponseDto>($"Ocurrió un error al crear la suplencia: {ex.Message}");
            }
            return response;
        }

        public async Task<ResponseDto<SubstitutionHistoryResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<SubstitutionHistoryResponseDto>();
            try
            {
                var entity = await _substitutionHistoryRepository.GetFirstOrDefaultAsync(
                    filter: x => x.SubstitutionHistoryId == id && x.IsActive,
                    includeProperties: [
                        x => x.PeriodAudit,
                        x => x.PeriodAudit.Store,
                        x => x.PeriodAudit.Store.Enterprise,
                        x => x.PreviousUserReference,
                        x => x.NewUserReference
                    ]
                );

                if (entity == null)
                {
                    response = ResponseDto.Error<SubstitutionHistoryResponseDto>("No se encontró la suplencia especificada.");
                    return response;
                }

                response.Data = _mapper.Map<SubstitutionHistoryResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la suplencia por ID");
                response = ResponseDto.Error<SubstitutionHistoryResponseDto>($"Ocurrió un error al obtener la suplencia: {ex.Message}");
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<SubstitutionHistoryResponseDto>>> GetPaged(SubstitutionHistoryFilterRequestDto filterDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<SubstitutionHistoryResponseDto>>();
            try
            {
                Expression<Func<SubstitutionHistory, bool>> filter = x => x.IsActive;

                // Filtro por ID de auditoría
                if (filterDto.PeriodAuditId.HasValue)
                {
                    filter = filter.AndAlso(x => x.PeriodAuditId == filterDto.PeriodAuditId.Value);
                }

                // Filtro por código de auditoría
                if (!string.IsNullOrEmpty(filterDto.AuditCode))
                {
                    filter = filter.AndAlso(x => x.PeriodAudit.CorrelativeNumber != null
                                                && x.PeriodAudit.CorrelativeNumber.Contains(filterDto.AuditCode));
                }

                // Filtro por nombre de usuario (anterior o nuevo)
                if (!string.IsNullOrEmpty(filterDto.UserName))
                {
                    filter = filter.AndAlso(x =>
                        (x.PreviousUserReference != null &&
                         (x.PreviousUserReference.FirstName.Contains(filterDto.UserName) ||
                          x.PreviousUserReference.LastName.Contains(filterDto.UserName)))
                        ||
                        (x.NewUserReference.FirstName.Contains(filterDto.UserName) ||
                         x.NewUserReference.LastName.Contains(filterDto.UserName))
                    );
                }

                // Filtro por ID de tienda
                if (filterDto.StoreId.HasValue)
                {
                    filter = filter.AndAlso(x => x.PeriodAudit.StoreId == filterDto.StoreId.Value);
                }

                // Filtro por ID de empresa
                if (filterDto.EnterpriseId.HasValue)
                {
                    filter = filter.AndAlso(x => x.PeriodAudit.Store != null
                                                && x.PeriodAudit.Store.EnterpriseId == filterDto.EnterpriseId.Value);
                }

                // Filtro general
                if (!string.IsNullOrEmpty(filterDto.Filter))
                {
                    filter = filter.AndAlso(x =>
                        x.RoleName.Contains(filterDto.Filter) ||
                        (x.ChangeReason != null && x.ChangeReason.Contains(filterDto.Filter)) ||
                        (x.PeriodAudit.CorrelativeNumber != null && x.PeriodAudit.CorrelativeNumber.Contains(filterDto.Filter))
                    );
                }

                Func<IQueryable<SubstitutionHistory>, IOrderedQueryable<SubstitutionHistory>> orderBy =
                    q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _substitutionHistoryRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: filterDto.PageNumber,
                    pageSize: filterDto.PageSize,
                    includeProperties: [
                        x => x.PeriodAudit,
                        x => x.PeriodAudit.Store,
                        x => x.PeriodAudit.Store.Enterprise,
                        x => x.PreviousUserReference,
                        x => x.NewUserReference
                    ]
                );

                var pagedResult = new PaginationResponseDto<SubstitutionHistoryResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<SubstitutionHistoryResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = filterDto.PageNumber,
                    PageSize = filterDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista paginada de suplencias");
                response = ResponseDto.Error<PaginationResponseDto<SubstitutionHistoryResponseDto>>(
                    $"Ocurrió un error al obtener la lista de suplencias: {ex.Message}");
            }
            return response;
        }
    }
}
