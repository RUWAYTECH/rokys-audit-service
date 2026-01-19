using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.PeriodAudit
{
  public class PeriodAuditActionPlanResponseDto : PeriodAuditActionPlanDto
  {
    public Guid PeriodAuditActionPlanId { get; set; }
  }
}