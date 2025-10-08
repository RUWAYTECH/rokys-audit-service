using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IUserReferenceRepository : IRepository<UserReference>
    {
        /// <summary>
        /// Busca un usuario por UserId del sistema de seguridad
        /// </summary>
        /// <param name="userId">ID del usuario en el sistema de seguridad</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<UserReference?> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Busca un usuario por EmployeeId del sistema de empleados
        /// </summary>
        /// <param name="employeeId">ID del empleado en el sistema de empleados</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<UserReference?> GetByEmployeeIdAsync(Guid employeeId);

        /// <summary>
        /// Busca un usuario por número de documento
        /// </summary>
        /// <param name="documentNumber">Número de documento</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<UserReference?> GetByDocumentNumberAsync(string documentNumber);

        /// <summary>
        /// Busca un usuario por correo electrónico
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <returns>Usuario encontrado o null</returns>
        Task<UserReference?> GetByEmailAsync(string email);

        /// <summary>
        /// Obtiene usuarios por código de rol
        /// </summary>
        /// <param name="roleCode">Código del rol</param>
        /// <returns>Lista de usuarios con el rol especificado</returns>
        Task<List<UserReference>> GetByRoleCodeAsync(string roleCode);

        /// <summary>
        /// Obtiene usuarios activos
        /// </summary>
        /// <returns>Lista de usuarios activos</returns>
        Task<List<UserReference>> GetActiveUsersAsync();

        /// <summary>
        /// Verifica si existe un usuario con el UserId especificado
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="excludeId">ID a excluir de la búsqueda (para updates)</param>
        /// <returns>True si existe, False en caso contrario</returns>
        Task<bool> ExistsByUserIdAsync(Guid userId, Guid? excludeId = null);

        /// <summary>
        /// Verifica si existe un usuario con el EmployeeId especificado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <param name="excludeId">ID a excluir de la búsqueda (para updates)</param>
        /// <returns>True si existe, False en caso contrario</returns>
        Task<bool> ExistsByEmployeeIdAsync(Guid employeeId, Guid? excludeId = null);
    }
}