using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.PeriodAudit;
using Rokys.Audit.DTOs.Responses.PeriodAuditScaleSubResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditScoringCriteriaResult;
using Rokys.Audit.DTOs.Responses.ScaleCompany;
using Rokys.Audit.DTOs.Responses.ScaleGroup;
using Rokys.Audit.DTOs.Responses.SubScale;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult
{
    public class PeriodAuditScaleResultCustomResponseDto : PeriodAuditScaleResultDto
    {
        public string ScaleType { get; set; } = string.Empty;
        public ScaleGroupPartialResponseDto ScaleGroup { get; set; }
        public List<ScaleCompanyResponseDto> ScaleCompany { get; set; } = new List<ScaleCompanyResponseDto>();
        public List<SubScaleResponseDto> SubScales { get; set; } = new List<SubScaleResponseDto>();
        public PeriodAuditPartialResponseDto PeriodAudit { get; set; }
        public List<PeriodAuditScoringCriteriaResultResponseDto> PeriodAuditScoringCriteriaResult { get; set; }
        public List<PeriodAuditScaleSubResultResponseDto> PeriodAuditScaleSubResult { get; set; }
    }
}
