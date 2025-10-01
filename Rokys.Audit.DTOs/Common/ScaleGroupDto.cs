using System.Text.RegularExpressions;

namespace Rokys.Audit.DTOs.Common
{
    public class ScaleGroupDto
    {
        public Guid GroupId { get; set; }
        public string Description { get; set; }
        public virtual Group Group { get; set; }
    }
}
