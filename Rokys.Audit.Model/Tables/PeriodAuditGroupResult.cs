using System;

namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditGroupResult : AuditEntity
    {
    public Guid PeriodAuditGroupResultId { get; set; }
    public Guid PeriodAuditId { get; set; }
    public Guid GroupId { get; set; }
    public decimal ScoreValue { get; set; }
    public int SortOrder { get; set; } = 0;
    public string? Observations { get; set; }
    public string? ScaleDescription { get; set; }
    public decimal TotalWeighting { get; set; }
    public virtual PeriodAudit? PeriodAudit { get; set; }
    public virtual Group? Group { get; set; }
    public string? ScaleColor { get; set; }
    public bool? HasEvidence { get; set; }
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<PeriodAuditScaleResult> PeriodAuditScaleResults { get; set; } = new List<PeriodAuditScaleResult>();
    }
}
