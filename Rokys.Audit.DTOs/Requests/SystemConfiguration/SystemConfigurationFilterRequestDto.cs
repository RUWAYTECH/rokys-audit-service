using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.SystemConfiguration
{
    public class SystemConfigurationFilterRequestDto : PaginationRequestDto
    {
        public string? Filter { get; set; }
        public string? ReferenceType { get; set; }
        public string? DataType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
