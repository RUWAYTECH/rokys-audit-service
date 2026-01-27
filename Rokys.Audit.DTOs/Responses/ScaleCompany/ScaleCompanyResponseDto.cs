using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.ScaleCompany
{
    public class ScaleCompanyResponseDto : ScaleCompanyDto
    {
        public Guid ScaleCompanyId { get; set; }
        public int SortOrder { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? EnterpriseName { get; set; }
        public string? EnterpriseGroupingName { get; set; }
        public List<string> EnterpriseNames { get; set; } = new List<string>();
    }
}
