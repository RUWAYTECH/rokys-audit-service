namespace Rokys.Audit.Model.Tables
{
    public class EvidenceFiles : AuditEntity
    {
        public Guid EvidenceFileId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditResultId { get; set; }
        public string OriginalName { get; set; } = string.Empty; // Nombre original del archivo
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty; // URL o path del archivo
        public string? FileType { get; set; } // Tipo de archivo (opcional)
        public string? UploadedBy { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAuditResult PeriodAuditResult { get; set; } = null!;
    }
}