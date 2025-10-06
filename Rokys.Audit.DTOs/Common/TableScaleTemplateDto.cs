
using System.Runtime.Serialization;

namespace Rokys.Audit.DTOs.Common;

public class TableScaleTemplateDto
{
    public Guid ScaleGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Orientation { get; set; }

    public string TemplateData { get; set; } = string.Empty;
}