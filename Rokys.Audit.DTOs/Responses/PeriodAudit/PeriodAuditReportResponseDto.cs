namespace Rokys.Audit.DTOs.Responses.PeriodAudit;

public class PeriodAuditReportResponseDto
{
    public string FileBase64 { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
}
