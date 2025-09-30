namespace Rokys.Audit.Model.Tables
{
    public class RiskScaleGroup : RiskCommonEntity
    {
        public Guid RiskScaleGroupId { get; set; } = Guid.NewGuid();
        public Guid ScaleGroupId { get; set; }
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ScaleGroup ScaleGroup { get; set; } = null!;
        public virtual ICollection<RiskScale> RiskScales { get; set; } = new List<RiskScale>();
    }
}