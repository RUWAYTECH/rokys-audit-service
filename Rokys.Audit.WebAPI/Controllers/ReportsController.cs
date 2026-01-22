using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.Reports;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    /// <summary>
    /// Controlador para reportes y dashboards
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [HttpGet("dashboards/evolutions/{year:int}")]
        public async Task<IActionResult> GetDashboardEvolutionsData([FromRoute] int year, [FromQuery] Guid[] enterpriseIds)
        {

            var response = await _reportsService.GetDashboardEvolutionsDataAsync(year, enterpriseIds);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("dashboards/supervisors/{year:int}")]
        public async Task<IActionResult> GetDashboardSupervisorsData([FromRoute] int year, [FromQuery] Guid[] enterpriseIds, [FromQuery] Guid[] supervisorIds)
        {

            var response = await _reportsService.GetDashboardSupervisorsDataAsync(year, enterpriseIds, supervisorIds);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("dashboards/stores/{year:int}")]
        public async Task<IActionResult> GetDashboardStoresData([FromRoute] int year, [FromQuery] Guid[] enterpriseIds, [FromQuery] Guid[] storeIds)
        {

            var response = await _reportsService.GetDashboardStoresDataAsync(year, enterpriseIds, storeIds);
            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetReportSearch([FromQuery] ReportSearchFilterRequestDto reportSearchFilterRequestDto)
        {

            var response = await _reportsService.GetReportSearchAsync(reportSearchFilterRequestDto);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportReport([FromQuery] ReportSearchFilterRequestDto reportSearchFilterRequestDto)
        {
            var response = await _reportsService.ExportReport(reportSearchFilterRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}