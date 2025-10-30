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
        public DateTime? ReportDate { get; set; }
    }
}