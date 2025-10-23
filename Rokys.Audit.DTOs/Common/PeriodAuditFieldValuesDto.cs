using System;
using System.Text.Json.Nodes;

namespace Rokys.Audit.DTOs.Common
{
    public class PeriodAuditFieldValuesDto
    {
        public Guid? AuditTemplateFieldId { get; set; }
        public Guid? PeriodAuditTableScaleTemplateResultId { get; set; }
        public string FieldCode { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public string IsCalculated { get; set; }
        public string? CalculationFormula { get; set; }
        public string? AcumulationType { get; set; }
        public string? FieldOptions { get; set; }
        public JsonArray? TableDataHorizontal { get; set; }
        public string? TextValue { get; set; }
        public decimal? NumericValue { get; set; }
        public string? DefaultValue { get; set; }
        public DateTime? DateValue { get; set; }
        public bool? BooleanValue { get; set; }
        public string? ImageUrl { get; set; }
        public string? FieldOptionsValue { get; set; }
        public string? ValidationStatus { get; set; }
        public string? ValidationMessage { get; set; }
        public int? SortOrder { get; set; }
    }
}