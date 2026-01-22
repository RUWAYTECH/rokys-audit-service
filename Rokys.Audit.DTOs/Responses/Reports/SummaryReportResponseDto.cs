using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rokys.Audit.DTOs.Responses.Reports
{
    public class SummaryReportResponseDto
    {
        public float Ranking { get; set; }
        public decimal ResultByMonth { get; set; }
        public string Risk { get; set; }
        public string RiskColor { get; set; }
        public int QuantityAudit { get; set; }
    }
}