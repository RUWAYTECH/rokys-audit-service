using System;

namespace Rokys.Audit.DTOs.Requests.ScaleGroup
{
    public class ChangeScaleGroupOrderRequestDto
    {
        public Guid GroupId { get; set; }
        public int CurrentPosition { get; set; }
        public int NewPosition { get; set; }
    }
}
