using System;

namespace Rokys.Audit.Model.Tables;

public class PeriodAuditActionPlan
{
    public Guid PeriodAuditActionPlanId { get; set; }
    public Guid DisiplinaryMeasureTypeId { get; set; }
    public Guid ResponsibleUserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool ApplyMeasure { get; set; }
    public DateTime CreationDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdateDate { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Navigation properties
    public virtual PeriodAuditScaleResult? PeriodAuditScaleResult { get; set; }
    public virtual MaintenanceDetailTable? DisiplinaryMeasureType { get; set; }
    public virtual UserReference? ResponsibleUser { get; set; }
}
