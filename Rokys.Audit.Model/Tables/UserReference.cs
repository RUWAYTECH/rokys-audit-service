namespace Rokys.Audit.Model.Tables
{
    /// <summary>
    /// Entidad de referencia de usuarios que conecta con el microservicio de seguridad y empleados
    /// </summary>
    public class UserReference: AuditEntity
    {
        /// <summary>
        /// ID de referencia del usuario (PK)
        /// </summary>
        public Guid UserReferenceId { get; set; }

        /// <summary>
        /// ID del usuario (desde Security MS)
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// ID del empleado (desde Memos MS)
        /// </summary>
        public Guid EmployeeId { get; set; }

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

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Usuario que creó el registro
        /// </summary>
      

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        // Navegación - Auditorías donde es Administrador
        public virtual ICollection<PeriodAudit>? AdministratorAudits { get; set; }

        // Navegación - Auditorías donde es Asistente
        public virtual ICollection<PeriodAudit>? AssistantAudits { get; set; }

        // Navegación - Auditorías donde es Gerente de Operación
        public virtual ICollection<PeriodAudit>? OperationManagerAudits { get; set; }

        // Navegación - Auditorías donde es Administrador Suplente
        public virtual ICollection<PeriodAudit>? FloatingAdministratorAudits { get; set; }

        // Navegación - Auditorías donde es Auditor Responsable
        public virtual ICollection<PeriodAudit>? ResponsibleAuditorAudits { get; set; }

        // Navegación - Stores asignadas
        public virtual ICollection<EmployeeStore>? EmployeeStores { get; set; }
    }
}