using Microsoft.AspNetCore.Http;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.ScaleGroup
{
    public class ScaleGroupRequestDto : ScaleGroupDto
    {
        public IFormFile? File { get; set; }
    }
}
