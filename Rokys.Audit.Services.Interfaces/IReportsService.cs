using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Reports;

namespace Rokys.Audit.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de reportes y dashboards
    /// </summary>
    public interface IReportsService
    {
        /// <summary>
        /// Obtiene los datos del dashboard por año
        /// </summary>
        /// <param name="year">Año para el cual obtener los datos</param>
        /// <returns>Datos estructurados para el dashboard</returns>
        Task<ResponseDto<DashboardDataResponseDto>> GetDashboardEvolutionsDataAsync(int year);
    }
}