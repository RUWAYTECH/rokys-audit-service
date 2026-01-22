namespace Rokys.Audit.DTOs.Requests.UserReference
{
    public class UserReferenceRequestDto
    {
        /// <summary>
        /// ID del usuario (desde Security MS)
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// ID del empleado (desde Memos MS)
        /// </summary>
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del usuario
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Correo electrónico personal
        /// </summary>
        public string? PersonalEmail { get; set; }

        /// <summary>
        /// Número de documento
        /// </summary>
        public string? DocumentNumber { get; set; }

        /// <summary>
        /// Código del rol
        /// </summary>
        public string? RoleCode { get; set; }

        /// <summary>
        /// Nombre del rol
        /// </summary>
        public string? RoleName { get; set; }
        public EmployeeStoreReferenceRequestDto[] EmployeeStores { get; init; } = Array.Empty<EmployeeStoreReferenceRequestDto>();
    }
}