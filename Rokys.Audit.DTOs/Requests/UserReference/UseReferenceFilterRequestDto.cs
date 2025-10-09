using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.Group
{
    public class UseReferenceFilterRequestDto : PaginationRequestDto
    {
        public string? RoleCode { get; set; }
    }
}
