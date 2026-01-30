using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Group;
using Rokys.Audit.DTOs.Requests.UserReference;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.UserReference;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IUserReferenceService : IBaseService<UserReferenceRequestDto, UserReferenceResponseDto>
    {
        /// <summary>
        /// Obtiene usuarios con paginación
        /// </summary>
        /// <param name="requestDto">Datos de paginación</param>
        /// <returns>Lista paginada de usuarios</returns>
        Task<ResponseDto<PaginationResponseDto<UserReferenceResponseDto>>> GetPaged(UseReferenceFilterRequestDto requestDto);

        /// <summary>
        /// Busca usuarios por UserId del sistema de seguridad
        /// </summary>
        /// <param name="userId">ID del usuario en el sistema de seguridad</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<ResponseDto<UserReferenceResponseDto?>> GetByUserId(Guid userId);

        /// <summary>
        /// Busca usuarios por EmployeeId del sistema de empleados
        /// </summary>
        /// <param name="employeeId">ID del empleado en el sistema de empleados</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<ResponseDto<UserReferenceResponseDto?>> GetByEmployeeId(Guid employeeId);

        /// <summary>
        /// Busca usuarios por número de documento
        /// </summary>
        /// <param name="documentNumber">Número de documento</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<ResponseDto<UserReferenceResponseDto?>> GetByDocumentNumber(string documentNumber);

        /// <summary>
        /// Busca usuarios por correo electrónico
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<ResponseDto<UserReferenceResponseDto?>> GetByEmail(string email);

        /// <summary>
        /// Obtiene usuarios por código de rol
        /// </summary>
        /// <param name="roleCode">Código del rol</param>
        /// <returns>Lista de usuarios con el rol especificado</returns>
        Task<ResponseDto<List<UserReferenceResponseDto>>> GetByRoleCode(string roleCode);

        /// <summary>
        /// Obtiene usuarios activos
        /// </summary>
        /// <returns>Lista de usuarios activos</returns>
        Task<ResponseDto<List<UserReferenceResponseDto>>> GetActiveUsers();

        Task<ResponseDto<UserReferenceResponseDto>> UpdateByUser(Guid userReferenceId, UserReferenceRequestDto requestDto);

        Task<ResponseDto<List<UserReferenceResponseDto>>> GetUsersByEnterpriseGroupingId(Guid enterpriseGroupingId);
    }
}