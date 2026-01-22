namespace Rokys.Audit.Model.Tables
{
    /// <summary>
    /// Entidad para registrar la historia de suplencias de usuarios en auditorías
    /// </summary>
    public class SubstitutionHistory : AuditEntity
    {
        /// <summary>
        /// ID del historial de suplencia (PK)
        /// </summary>
        public Guid SubstitutionHistoryId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// ID de la auditoría asociada
        /// </summary>
        public Guid PeriodAuditId { get; set; }

        /// <summary>
        /// Nombre del rol en el contexto de la auditoría (informativo, no cambia)
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// ID del usuario anterior (obligatorio para suplencias)
        /// </summary>
        public Guid PreviousUserReferenceId { get; set; }

        /// <summary>
        /// ID del nuevo usuario asignado (obligatorio)
        /// </summary>
        public Guid NewUserReferenceId { get; set; }

        /// <summary>
        /// Razón del cambio de usuario
        /// </summary>
        public string? ChangeReason { get; set; }

        /// <summary>
        /// Indica si el registro está activo
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// Auditoría asociada
        /// </summary>
        public virtual PeriodAudit? PeriodAudit { get; set; }

        /// <summary>
        /// Usuario anterior
        /// </summary>
        public virtual UserReference? PreviousUserReference { get; set; }

        /// <summary>
        /// Nuevo usuario asignado
        /// </summary>
        public virtual UserReference? NewUserReference { get; set; }
    }
}
