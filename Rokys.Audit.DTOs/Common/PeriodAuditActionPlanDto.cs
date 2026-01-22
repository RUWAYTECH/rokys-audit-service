using System;
using System.Collections.Generic;

namespace Rokys.Audit.DTOs.Common
{
  public class PeriodAuditActionPlanDto
  {
    public Guid PeriodAuditScaleResultId { get; set; }
    public Guid? DisiplinaryMeasureTypeId { get; set; }
    public Guid ResponsibleUserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool ApplyMeasure { get; set; }
  }
}