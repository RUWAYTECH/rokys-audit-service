using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditTemplateField;
using Rokys.Audit.DTOs.Responses.AuditTemplateField;
using Rokys.Audit.DTOs.Responses.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IAuditTemplateFieldService : IBaseService<AuditTemplateFieldRequestDto, AuditTemplateFieldResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<AuditTemplateFieldResponseDto>>> GetPaged(AuditTemplateFieldFilterRequestDto requestDto);
    }
}
