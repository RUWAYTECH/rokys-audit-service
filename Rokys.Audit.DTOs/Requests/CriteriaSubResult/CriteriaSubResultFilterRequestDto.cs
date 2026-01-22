using Rokys.Audit.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.DTOs.Requests.CriteriaSubResult
{
    public class CriteriaSubResultFilterRequestDto : PaginationRequestDto
    {
        public Guid? ScaleGroupId { get; set; }
    }
}
