using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.UserReference
{
    public class UserReferenceResponseDto : UserReferenceDto
    {
        /// <summary>
        /// ID de referencia del usuario (PK)
        /// </summary>
        public Guid UserReferenceId { get; set; }

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Usuario que creó el registro
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Usuario que actualizó el registro
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}