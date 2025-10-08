using System.ComponentModel.DataAnnotations;

namespace Rokys.Audit.DTOs.Requests.UserReference
{
    public class UserReferenceRequestDto
    {
        /// <summary>
        /// ID del usuario (desde Security MS)
        /// </summary>
        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public Guid UserId { get; set; }

        /// <summary>
        /// ID del empleado (desde Memos MS)
        /// </summary>
        [Required(ErrorMessage = "El ID del empleado es requerido")]
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del usuario
        /// </summary>
        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(200, ErrorMessage = "El apellido no puede exceder los 200 caracteres")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico
        /// </summary>
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder los 150 caracteres")]
        public string? Email { get; set; }

        /// <summary>
        /// Número de documento
        /// </summary>
        [StringLength(20, ErrorMessage = "El número de documento no puede exceder los 20 caracteres")]
        public string? DocumentNumber { get; set; }

        /// <summary>
        /// Código del rol
        /// </summary>
        [StringLength(50, ErrorMessage = "El código del rol no puede exceder los 50 caracteres")]
        public string? RoleCode { get; set; }

        /// <summary>
        /// Nombre del rol
        /// </summary>
        [StringLength(100, ErrorMessage = "El nombre del rol no puede exceder los 100 caracteres")]
        public string? RoleName { get; set; }

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}