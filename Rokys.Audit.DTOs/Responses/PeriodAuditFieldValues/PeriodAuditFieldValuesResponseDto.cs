using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditFieldValues
{
    public class PeriodAuditFieldValuesResponseDto : PeriodAuditFieldValuesDto
    {
        public Guid PeriodAuditFieldValueId { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
    }
}