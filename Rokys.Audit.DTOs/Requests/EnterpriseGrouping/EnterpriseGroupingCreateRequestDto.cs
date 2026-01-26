using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.DTOs.Requests.EnterpriseGrouping
{
    public class EnterpriseGroupingCreateRequestDto : EnterpriseGroupingRequestDto
    {
        public List<Guid> EnterpriseIds { get; set; } = new();
    }
}
