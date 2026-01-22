namespace Rokys.Audit.DTOs.Responses.SubstitutionHistory
{
    public class SubstitutionHistoryResponseDto
    {
        public Guid SubstitutionHistoryId { get; set; }
        public Guid PeriodAuditId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public Guid PreviousUserReferenceId { get; set; }
        public Guid NewUserReferenceId { get; set; }
        public string? ChangeReason { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Navigation properties for display
        /// <summary>
        /// Código de auditoría
        /// </summary>
        public string? AuditCode { get; set; }

        /// <summary>
        /// Nombre de la tienda
        /// </summary>
        public string? StoreName { get; set; }

        /// <summary>
        /// Nombre de la empresa
        /// </summary>
        public string? EnterpriseName { get; set; }

        /// <summary>
        /// Nombre completo del usuario anterior
        /// </summary>
        public string? PreviousUserFullName { get; set; }

        /// <summary>
        /// Email del usuario anterior
        /// </summary>
        public string? PreviousUserEmail { get; set; }

        /// <summary>
        /// Nombre completo del nuevo usuario
        /// </summary>
        public string? NewUserFullName { get; set; }

        /// <summary>
        /// Email del nuevo usuario
        /// </summary>
        public string? NewUserEmail { get; set; }
    }
}
