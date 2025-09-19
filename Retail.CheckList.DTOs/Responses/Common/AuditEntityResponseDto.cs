using System;

namespace Rokys.Audit.DTOs.Responses.Common
{
    public class AuditEntityResponseDto
    {
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
