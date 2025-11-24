using System;

namespace Rokys.Audit.DTOs.Requests.Group
{
    public class ChangeGroupOrderRequestDto
    {
        public Guid EnterpriseId { get; set; }
        public int CurrentPosition { get; set; }
        public int NewPosition { get; set; }
    }
}
