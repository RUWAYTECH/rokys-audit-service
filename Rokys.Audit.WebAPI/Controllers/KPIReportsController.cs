using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.KpiReports;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/kpi-reports")]
    public class KPIReportsController : ControllerBase
    {
        private readonly IKPIReportsService _kpiReportsService;

        public KPIReportsController(IKPIReportsService kpiReportsService)
        {
            _kpiReportsService = kpiReportsService;
        }

        [HttpGet("general/{year:int}")]
        public async Task<IActionResult> GetGeneralKPIs([FromRoute] int year, [FromQuery] Guid[] enterpriseIds, [FromQuery] Guid? enterpriseGroupingId)
        {
            var response = await _kpiReportsService.GetGeneralKPIsAsync(year, enterpriseIds, enterpriseGroupingId);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("data-by-participant")]
        public async Task<IActionResult> GetDataByParticipant([FromQuery] DataByParticipantRequestDto request)
        {
            var response = await _kpiReportsService.GetDataByParticipantAsync(request);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("data-by-auditable-group")]
        public async Task<IActionResult> GetDataByAuditableGroup([FromQuery] DataByAuditableGroupRequestDto request)
        {
            var response = await _kpiReportsService.GetDataByAuditableGroupAsync(request);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("data-by-store")]
        public async Task<IActionResult> GetDataByStore([FromQuery] DataByStoreRequestDto request)
        {
            var response = await _kpiReportsService.GetDataByStoreAsync(request);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("data-by-supervisor-store")]
        public async Task<IActionResult> GetDataBySupervisorStore([FromQuery] DataBySupervisorStoreRequestDto request)
        {
            var response = await _kpiReportsService.GetDataBySupervisorStoreAsync(request);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("data-by-scale")]
        public async Task<IActionResult> GetDataByScale([FromQuery] DataByScaleRequestDto request)
        {
            var response = await _kpiReportsService.GetDataByScaleAsync(request);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("general-by-kpis-filter")]
        public async Task<IActionResult> GetGeneralByKPIsFilter([FromQuery] DataByFilterGeneralRequestDto request)
        {
            var response = await _kpiReportsService.GetGeneralByKPIsFilterAsync(request);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
