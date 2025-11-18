using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.SubstitutionHistory;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.SubstitutionHistory;

namespace Rokys.Audit.Services.Interfaces
{
    public interface ISubstitutionHistoryService
    {
        /// <summary>
        /// Registra una nueva suplencia. Valida que ambos usuarios pertenezcan al mismo rol
        /// y que la auditoría esté en estado "En proceso"
        /// </summary>
        Task<ResponseDto<SubstitutionHistoryResponseDto>> Create(SubstitutionHistoryRequestDto requestDto);

        /// <summary>
        /// Lista las suplencias con paginación y filtros
        /// </summary>
        Task<ResponseDto<PaginationResponseDto<SubstitutionHistoryResponseDto>>> GetPaged(SubstitutionHistoryFilterRequestDto requestDto);

        /// <summary>
        /// Obtiene una suplencia por su ID
        /// </summary>
        Task<ResponseDto<SubstitutionHistoryResponseDto>> GetById(Guid id);
    }
}
