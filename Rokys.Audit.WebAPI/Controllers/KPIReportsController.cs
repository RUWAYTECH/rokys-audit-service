using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
