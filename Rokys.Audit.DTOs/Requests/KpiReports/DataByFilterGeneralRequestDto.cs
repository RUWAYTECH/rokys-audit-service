using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.DTOs.Requests.KpiReports
{
    public class DataByFilterGeneralRequestDto
    {
        public Guid[]? StoreIds { get; set; }
        public Guid[]? SupervisorIds { get; set; }
        public Guid[]? UnitManagerIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
