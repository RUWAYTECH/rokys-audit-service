using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.DTOs.Requests.Enterprise
{
    public class EnterpriseCreateRequestDto : EnterpriseRequestDto
    {
        public Guid? EnterpriseId { get; set; }
    }
}
