using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.UserReference;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.UserReference;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class UserReferenceService : IUserReferenceService
    {
        private readonly IUserReferenceRepository _userReferenceRepository;
        private readonly IValidator<UserReferenceRequestDto> _fluentValidator;
        private readonly ILogger<UserReferenceService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserReferenceService(
            IUserReferenceRepository userReferenceRepository,
            IValidator<UserReferenceRequestDto> fluentValidator,
            ILogger<UserReferenceService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _userReferenceRepository = userReferenceRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<UserReferenceResponseDto>> Create(UserReferenceRequestDto requestDto)
        {
            var response = ResponseDto.Create<UserReferenceResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<UserReference>(requestDto);
                entity.CreateAudit(currentUser.UserName);

                _userReferenceRepository.Insert(entity);
                await _unitOfWork.CommitAsync();

                response.Data = _mapper.Map<UserReferenceResponseDto>(entity);
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Usuario creado exitosamente",
                    MessageType = ApplicationMessageType.Success
                });

                _logger.LogInformation("UserReference created successfully with ID: {UserReferenceId}", entity.UserReferenceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating UserReference");
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al crear el usuario",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto<UserReferenceResponseDto>> Update(Guid id, UserReferenceRequestDto requestDto)
        {
            var response = ResponseDto.Create<UserReferenceResponseDto>();
            try
            {
                var entity = await _userReferenceRepository.GetByKeyAsync(id);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "Usuario no encontrado",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                // Validación personalizada para update (excluir el ID actual)
                var existsUserId = await _userReferenceRepository.ExistsByUserIdAsync(requestDto.UserId, id);
                if (existsUserId)
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "Ya existe otro usuario con este ID del sistema de seguridad",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                var existsEmployeeId = await _userReferenceRepository.ExistsByEmployeeIdAsync(requestDto.EmployeeId, id);
                if (existsEmployeeId)
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "Ya existe otro usuario con este ID del sistema de empleados",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                // Mapear los cambios
                _mapper.Map(requestDto, entity);
                entity.UpdatedBy = currentUser.UserName;
                entity.UpdateDate = DateTime.UtcNow;

                _userReferenceRepository.Update(entity);
                await _unitOfWork.CommitAsync();

                response.Data = _mapper.Map<UserReferenceResponseDto>(entity);
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Usuario actualizado exitosamente",
                    MessageType = ApplicationMessageType.Success
                });

                _logger.LogInformation("UserReference updated successfully with ID: {UserReferenceId}", entity.UserReferenceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating UserReference with ID: {UserReferenceId}", id);
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al actualizar el usuario",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _userReferenceRepository.GetByKeyAsync(id);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "Usuario no encontrado",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                // Soft delete
                entity.IsActive = false;
                entity.UpdatedBy = currentUser.UserName;
                entity.UpdateDate = DateTime.UtcNow;

                _userReferenceRepository.Update(entity);
                await _unitOfWork.CommitAsync();

                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Usuario eliminado exitosamente",
                    MessageType = ApplicationMessageType.Success
                });

                _logger.LogInformation("UserReference soft deleted successfully with ID: {UserReferenceId}", entity.UserReferenceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting UserReference with ID: {UserReferenceId}", id);
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al eliminar el usuario",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto<UserReferenceResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<UserReferenceResponseDto>();
            try
            {
                var entity = await _userReferenceRepository.GetByKeyAsync(id);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "Usuario no encontrado",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                response.Data = _mapper.Map<UserReferenceResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UserReference with ID: {UserReferenceId}", id);
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al obtener el usuario",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto<List<UserReferenceResponseDto>>> GetAll()
        {
            var response = ResponseDto.Create<List<UserReferenceResponseDto>>();
            try
            {
                var entities = await _userReferenceRepository.GetAllAsync();
                response.Data = _mapper.Map<List<UserReferenceResponseDto>>(entities.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all UserReferences");
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al obtener los usuarios",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<UserReferenceResponseDto>>> GetPaged(PaginationRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<UserReferenceResponseDto>>();
            try
            {
                Expression<Func<UserReference, bool>> filter = x => x.IsActive;

                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                {
                    var searchTerm = paginationRequestDto.Filter.ToLower();
                    filter = x => x.IsActive && (x.FirstName.ToLower().Contains(searchTerm) ||
                                  x.LastName.ToLower().Contains(searchTerm) ||
                                  (x.Email != null && x.Email.ToLower().Contains(searchTerm)) ||
                                  (x.DocumentNumber != null && x.DocumentNumber.ToLower().Contains(searchTerm)) ||
                                  (x.RoleName != null && x.RoleName.ToLower().Contains(searchTerm)));
                }

                Func<IQueryable<UserReference>, IOrderedQueryable<UserReference>> orderBy = q => q.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);

                var entities = await _userReferenceRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize);

                var pagedResult = new PaginationResponseDto<UserReferenceResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<UserReferenceResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged UserReferences");
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al obtener los usuarios paginados",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto<UserReferenceResponseDto?>> GetByUserId(Guid userId)
        {
            var response = ResponseDto.Create<UserReferenceResponseDto?>();
            try
            {
                var entity = await _userReferenceRepository.GetFirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
                response.Data = entity != null ? _mapper.Map<UserReferenceResponseDto>(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UserReference by UserId: {UserId}", userId);
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al buscar el usuario por ID de seguridad",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto<UserReferenceResponseDto?>> GetByEmployeeId(Guid employeeId)
        {
            var response = ResponseDto.Create<UserReferenceResponseDto?>();
            try
            {
                var entity = await _userReferenceRepository.GetFirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.IsActive);
                response.Data = entity != null ? _mapper.Map<UserReferenceResponseDto>(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UserReference by EmployeeId: {EmployeeId}", employeeId);
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al buscar el usuario por ID de empleado",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto<UserReferenceResponseDto?>> GetByDocumentNumber(string documentNumber)
        {
            var response = ResponseDto.Create<UserReferenceResponseDto?>();
            try
            {
                if (string.IsNullOrWhiteSpace(documentNumber))
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "El número de documento es requerido",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                var entity = await _userReferenceRepository.GetFirstOrDefaultAsync(x => x.DocumentNumber == documentNumber && x.IsActive);
                response.Data = entity != null ? _mapper.Map<UserReferenceResponseDto>(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UserReference by DocumentNumber: {DocumentNumber}", documentNumber);
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al buscar el usuario por documento",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto<UserReferenceResponseDto?>> GetByEmail(string email)
        {
            var response = ResponseDto.Create<UserReferenceResponseDto?>();
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "El correo electrónico es requerido",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                var entity = await _userReferenceRepository.GetFirstOrDefaultAsync(x => x.Email == email && x.IsActive);
                response.Data = entity != null ? _mapper.Map<UserReferenceResponseDto>(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UserReference by Email: {Email}", email);
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al buscar el usuario por correo",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto<List<UserReferenceResponseDto>>> GetByRoleCode(string roleCode)
        {
            var response = ResponseDto.Create<List<UserReferenceResponseDto>>();
            try
            {
                if (string.IsNullOrWhiteSpace(roleCode))
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "El código del rol es requerido",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                var entities = await _userReferenceRepository.GetAsync(x => x.RoleCode == roleCode && x.IsActive);
                response.Data = _mapper.Map<List<UserReferenceResponseDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UserReferences by RoleCode: {RoleCode}", roleCode);
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al obtener usuarios por rol",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }

        public async Task<ResponseDto<List<UserReferenceResponseDto>>> GetActiveUsers()
        {
            var response = ResponseDto.Create<List<UserReferenceResponseDto>>();
            try
            {
                var entities = await _userReferenceRepository.GetAllAsync(x => x.IsActive);
                response.Data = _mapper.Map<List<UserReferenceResponseDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active UserReferences");
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error interno del servidor al obtener usuarios activos",
                    MessageType = ApplicationMessageType.Error
                });
            }

            return response;
        }
    }
}