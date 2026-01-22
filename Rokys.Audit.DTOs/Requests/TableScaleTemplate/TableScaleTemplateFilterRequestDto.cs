using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.TableScaleTemplate
{
    public class TableScaleTemplateFilterRequestDto: PaginationRequestDto
    {
        public Guid? ScaleGroupId { get; set; }
    
    }
}