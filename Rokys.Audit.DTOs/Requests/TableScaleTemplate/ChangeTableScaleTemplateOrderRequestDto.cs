using System;

namespace Rokys.Audit.DTOs.Requests.TableScaleTemplate
{
    public class ChangeTableScaleTemplateOrderRequestDto
    {
        public Guid ScaleGroupId { get; set; }
        public int CurrentPosition { get; set; }
        public int NewPosition { get; set; }
    }
}
