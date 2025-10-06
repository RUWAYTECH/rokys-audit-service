namespace Rokys.Audit.DTOs.Common
{
    public class AuditTemplateFieldDto
    {
        public Guid TableScaleTemplateId { get; set; }
        public string? FieldCode { get; set; }
        public string? FieldName { get; set; }
        public string? FieldType { get; set; }
        public bool IsCalculated { get; set; }
        public string? CalculationFormula { get; set; }
        public string? AcumulationType { get; set; }
        public string? FieldOptions { get; set; }
        public string? DefaultValue { get; set; }
    }
}
