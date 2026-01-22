using System;

namespace Rokys.Audit.Model.Tables;

public class PeriodAuditActionPlan: AuditEntity
{
    public Guid PeriodAuditActionPlanId { get; set; } = Guid.NewGuid();
    public Guid PeriodAuditScaleResultId { get; set; }
    public Guid? DisiplinaryMeasureTypeId { get; set; }
    public Guid ResponsibleUserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool ApplyMeasure { get; set; }

    // Navigation properties
    public virtual PeriodAuditScaleResult? PeriodAuditScaleResult { get; set; }
    public virtual MaintenanceDetailTable? DisiplinaryMeasureType { get; set; }
    public virtual UserReference? ResponsibleUser { get; set; }
}
