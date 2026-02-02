using System;

namespace Rokys.Audit.Model.Tables
{
    public class AuditPeriodPreEvaluation : AuditEntity
    {
        public Guid AuditPeriodPreEvaluationId { get; set; }
        public Guid PeriodAuditGroupResultId { get; set; }
        public decimal TotalWeighted { get; set; }
        public string? ScaleValueJSON { get; set; }
        public decimal TotalAcumulation { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual PeriodAuditGroupResult PeriodAuditGroupResult { get; set; } = null!;
    }
}
