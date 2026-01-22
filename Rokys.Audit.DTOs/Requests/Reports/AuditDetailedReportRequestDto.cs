using System;
using System.Collections.Generic;

namespace Rokys.Audit.DTOs.Requests.Reports
{
    /// <summary>
    /// DTO para solicitar reporte detallado de resultados de auditoría
    /// Permite filtrar por múltiples empresas, tiendas y grupos
    /// </summary>
    public class AuditDetailedReportRequestDto
    {
        /// <summary>
        /// Lista de IDs de empresas para filtrar
        /// </summary>
        public List<Guid>? EnterpriseIds { get; set; }

        /// <summary>
        /// Lista de IDs de tiendas para filtrar
        /// </summary>
        public List<Guid>? StoreIds { get; set; }

        /// <summary>
        /// Lista de IDs de grupos para filtrar
        /// </summary>
        public List<Guid>? GroupIds { get; set; }

        public string? AuditStatusCode { get; set; }
    }
}
