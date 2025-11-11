using System;
using System.Collections.Generic;

namespace Rokys.Audit.DTOs.Common
{
    public class PeriodAuditDto
    {
        public Guid StoreId { get; set; }
        public Guid? AdministratorId { get; set; }
        public Guid? AssistantId { get; set; }
        public Guid? OperationManagersId { get; set; }
        public Guid? FloatingAdministratorId { get; set; }
        public Guid? ResponsibleAuditorId { get; set; }
        public Guid? SupervisorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ReportDate { get; set; }
        public int? AuditedDays { get; set; }
        public Guid? StatusId { get; set; }
        public string? ScaleCode { get; set; }
        
        // Lista de participantes en la auditoría
        public List<PeriodAuditParticipantDto> Participants { get; set; } = new List<PeriodAuditParticipantDto>();
    }
}