namespace Rokys.Audit.DTOs.Requests.GroupingUser
{
    public class GroupingUserUpsertRequestDto
    {
        public Guid EnterpriseGroupingId { get; set; }
        public List<Guid> UserReferenceIds { get; set; }
        public string RolesCodes { get; set; } = string.Empty;
    }
}
