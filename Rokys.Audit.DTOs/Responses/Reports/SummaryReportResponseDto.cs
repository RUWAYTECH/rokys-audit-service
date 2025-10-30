using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rokys.Audit.DTOs.Responses.Reports
{
    public class SummaryReportResponseDto
    {
        public float Ranking { get; set; }
        public string ResultByMonth { get; set; }
        public string Risk { get; set; }
        public int QuantityAudit { get; set; }
    }
}