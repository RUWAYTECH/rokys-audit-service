using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues
{
    public class UpdatePeriodAuditFieldValuesRequestDto : PeriodAuditFieldValuesDto
    {
        public Guid PeriodAuditFieldValueId { get; set; }
    }
}
