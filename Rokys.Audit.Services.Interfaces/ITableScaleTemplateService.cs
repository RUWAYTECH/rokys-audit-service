using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.TableScaleTemplate;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.TableScaleTemplate;

namespace Rokys.Audit.Services.Interfaces
{
    public interface ITableScaleTemplateService
    {
        Task<ResponseDto<TableScaleTemplateResponseDto>> Create(TableScaleTemplateRequestDto requestDto);
        Task<ResponseDto<TableScaleTemplateResponseDto>> Update(Guid id, TableScaleTemplateRequestDto requestDto);
        Task<ResponseDto> Delete(Guid id);
        Task<ResponseDto<TableScaleTemplateResponseDto>> GetById(Guid id);
    Task<ResponseDto<PaginationResponseDto<TableScaleTemplateResponseDto>>> GetPaged(TableScaleTemplateFilterRequestDto filterRequestDto);
        Task<ResponseDto<IEnumerable<TableScaleTemplateResponseDto>>> GetByScaleGroupId(Guid scaleGroupId);
        Task<ResponseDto<bool>> ChangeOrder(Guid scaleGroupId, int currentPosition, int newPosition);
    }
}