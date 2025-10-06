using System;

namespace Rokys.Audit.DTOs.Common
{
    public class PeriodAuditDto
    {
        public Guid PeriodAuditId { get; set; }
        public Guid StoreId { get; set; }
        public Guid? AdministratorId { get; set; }
        public Guid? AssistantId { get; set; }
        public Guid? OperationManagersId { get; set; }
        public Guid? FloatingAdministratorId { get; set; }
        public Guid? ResponsibleAuditorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ReportDate { get; set; }
        public int? AuditedDays { get; set; }
        public string GlobalObservations { get; set; } = string.Empty;
        public decimal TotalWeighting { get; set; }
        public Guid StatusId { get; set; }
        public decimal ScoreValue { get; set; }
        public string ScaleName { get; set; } = string.Empty;
        public string ScaleIcon { get; set; } = string.Empty;
        public string ScaleColor { get; set; } = string.Empty;
        public decimal ScaleMinValue { get; set; }
        public decimal ScaleMaxValue { get; set; }
    }
}