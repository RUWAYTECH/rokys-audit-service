using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetDashboardEvolutionsData([FromRoute] int year)
        {
           
            var response = await _reportsService.GetDashboardEvolutionsDataAsync(year);
            
            if (response.IsValid)
                return Ok(response);
            
            return BadRequest(response);
        }
    }
}