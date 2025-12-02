using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.Reports
{
    public class ReportSearchFilterRequestDto
    {
        public Guid? EnterpriseId { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? SupervisorId { get; set; }
        public Guid? ResponsibleAuditorId { get; set; }
        public string? ReportDate { get; set; }  // Cambiar a string para soportar formato "YYYY-MM" o "YYYY-MM-DD"
        public DateTime? ReportDateInit { get; set; }
        public DateTime? ReportDateFinish { get; set; }
    }
}