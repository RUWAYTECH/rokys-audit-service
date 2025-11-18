using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.SubstitutionHistory
{
    public class SubstitutionHistoryFilterRequestDto : PaginationRequestDto
    {
        /// <summary>
        /// Filtro por código de auditoría (CorrelativeNumber)
        /// </summary>
        public string? AuditCode { get; set; }

        /// <summary>
        /// Filtro por nombre de usuario (anterior o nuevo)
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Filtro por ID de tienda
        /// </summary>
        public Guid? StoreId { get; set; }

        /// <summary>
        /// Filtro por ID de empresa
        /// </summary>
        public Guid? EnterpriseId { get; set; }

        /// <summary>
        /// Filtro por ID de auditoría
        /// </summary>
        public Guid? PeriodAuditId { get; set; }
    }
}
