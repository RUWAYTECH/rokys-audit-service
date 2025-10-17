using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.PeriodAudit;
using Rokys.Audit.DTOs.Responses.ScaleGroup;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult
{
    public class PeriodAuditScaleResultCustomResponseDto : PeriodAuditScaleResultDto
    {
        public ScaleGroupPartialResponseDto ScaleGroup { get; set; }
        public PeriodAuditPartialResponseDto PeriodAudit { get; set; }
    }
}
