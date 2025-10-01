namespace Rokys.Audit.Model.Tables
{
    public class ScaleGroup : RiskCommonEntity
    {
        public Guid ScaleGroupId { get; set; } = Guid.NewGuid();
        public Guid GroupId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Group Group { get; set; } = null!;
    }
}