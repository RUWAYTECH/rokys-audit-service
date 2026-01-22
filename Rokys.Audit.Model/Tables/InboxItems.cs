using System;

namespace Rokys.Audit.Model.Tables
{
    public class InboxItems : AuditEntity
    {
        public Guid InboxItemId { get; set; } = Guid.NewGuid();
            public Guid? PeriodAuditId { get; set; }
            public Guid? PrevStatusId { get; set; }
            public Guid? NextStatusId { get; set; }
            public Guid? PrevUserId { get; set; }
            public Guid? NextUserId { get; set; }
            public Guid? ApproverId { get; set; }
            public string? Comments { get; set; }
            // quien registró la acción en este inbox
            public Guid? UserId { get; set; }
            // acción ejecutada: 'Aprobada','Cancelada','Devuelta', etc.
            public string? Action { get; set; }
        // Secuencia incremental por PeriodAudit para saber el último creado
        public int SequenceNumber { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public virtual UserReference? User { get; set; }

        // Navigation to status snapshots (previous/next) — configured in AuditStatusConfig
        public virtual AuditStatus? PrevStatus { get; set; }
        public virtual AuditStatus? NextStatus { get; set; }
    }
}
