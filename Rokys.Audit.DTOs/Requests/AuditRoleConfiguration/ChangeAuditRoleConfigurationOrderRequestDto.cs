namespace Rokys.Audit.DTOs.Requests.AuditRoleConfiguration
{
    public class ChangeAuditRoleConfigurationOrderRequestDto
    {
        public int CurrentPosition { get; set; }
        public int NewPosition { get; set; }
        public Guid? EnterpriseId { get; set; }
    }
}