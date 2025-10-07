using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.PeriodAudit
{
    public class PeriodAuditResponseDto : PeriodAuditDto
    {
        
        public decimal ScoreValue { get; set; }
        public string ScaleName { get; set; } = string.Empty;
        public string ScaleIcon { get; set; } = string.Empty;
        public string ScaleColor { get; set; } = string.Empty;
        public decimal ScaleMinValue { get; set; }
        public decimal ScaleMaxValue { get; set; }
        public decimal TotalWeighting { get; set; }
        public string? GlobalObservations { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
    }
}