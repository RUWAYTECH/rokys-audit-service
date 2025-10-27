using Rokys.Audit.Common.Helpers.FileConvert;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.ScaleGroup
{
    public class ScaleGroupPartialResponseDto : ScaleGroupDto
    {
        public Guid ScaleGroupId { get; set; }
        public bool IsActive { get; set; }
        public DataSourceFiles? DataSourceTemplate { get; set; }
        public DataSourceFiles? DataSource { get; set; }

    }
    public class DataSourceFiles
    {
        public string FileName { get; set; }
        public string ClassificationType { get; set; }
        public FileBase64Result? DataSourceFile { get; set; }
    }
}
