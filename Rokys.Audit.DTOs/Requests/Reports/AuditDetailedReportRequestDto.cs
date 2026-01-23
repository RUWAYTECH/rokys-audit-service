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
        /// Lista de IDs de estados de auditoría para filtrar
        /// </summary>
        public List<Guid>? StatusIds { get; set; }

        /// <summary>
        /// Fechas de auditoría para filtrar (fecha de creación de la auditoría)
        /// </summary>
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
