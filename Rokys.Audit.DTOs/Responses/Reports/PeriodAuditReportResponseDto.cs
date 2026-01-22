using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rokys.Audit.DTOs.Responses.Reports
{
    public class PeriodAuditReportResponseDto
    {
        public List<SummaryReportResponseDto> Summaries { get; set; }
        public List<PeriodAuditItemReportResponseDto> Items { get; set; }

    }
}