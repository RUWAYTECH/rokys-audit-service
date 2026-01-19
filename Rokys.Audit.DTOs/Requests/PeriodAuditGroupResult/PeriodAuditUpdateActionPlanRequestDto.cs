using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult
{
  public class PeriodAuditUpdateActionPlanRequestDto
  {
    public List<PeriodAuditActionPlanDto> PeriodAuditActionPlans { get; set; } = new List<PeriodAuditActionPlanDto>();
  }
}