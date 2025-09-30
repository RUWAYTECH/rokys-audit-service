namespace Rokys.Audit.Model.Tables
{
    public class RiskScale : RiskCommonEntity
    {
        public int RiskScaleId { get; set; }
        public Guid RiskScaleGroupId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }

        public bool NonToleratedRisk { get; set; } = false;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual RiskScaleGroup RiskScaleGroup { get; set; } = null!;
    }
}