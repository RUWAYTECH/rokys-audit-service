using System;

namespace Rokys.Audit.DTOs.Requests.AuditTemplateField
{
    public class AuditTemplateFieldOrderRequestDto
    {
        public Guid TableScaleTemplateId { get; set; }
        public int CurrentPosition { get; set; }
        public int NewPosition { get; set; }
    }
}
