namespace Rokys.Audit.DTOs.Common
{
    public class GroupingUserDto
    {
        public Guid EnterpriseGroupingId { get; set; }
        public Guid UserReferenceId { get; set; }
        public string RolesCodes { get; set; } = string.Empty;
    }
}
