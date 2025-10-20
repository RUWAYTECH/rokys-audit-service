using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Group;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Group;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IGroupService : IBaseService<GroupRequestDto, GroupResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<GroupResponseDto>>> GetPaged(GroupFilterRequestDto requestDto);
        
        /// <summary>
        /// Clona un grupo y todas sus entidades hijas (ScaleGroup, TableScaleTemplate, AuditTemplateField, ScoringCriteria, CriteriaSubResult)
        /// </summary>
        /// <param name="requestDto">Datos para la clonación del grupo</param>
        /// <returns>Información del grupo clonado y estadísticas de clonación</returns>
        Task<ResponseDto<GroupCloneResponseDto>> CloneGroupAsync(GroupCloneRequestDto requestDto);
    }
}