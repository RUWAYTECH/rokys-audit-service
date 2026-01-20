using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/period-audit-action-plan")]
    public class PeriodAuditActionPlanController : ControllerBase
    {
        private readonly IPeriodAuditActionPlanService _periodAuditActionPlanService;

        public PeriodAuditActionPlanController(IPeriodAuditActionPlanService periodAuditActionPlanService)
        {
            _periodAuditActionPlanService = periodAuditActionPlanService;
        }

        /// <summary>
        /// Obtiene la configuración de la empresa asociada a una auditoría específica
        /// </summary>
        /// <param name="periodAuditId">ID de la auditoría del periodo</param>
        /// <returns>Configuración de la empresa si existe</returns>
        [HttpGet("enterprise-configuration/{periodAuditId}")]
        public async Task<IActionResult> GetEnterpriseConfigurationByPeriodAuditId([FromRoute] Guid periodAuditId)
        {
            var response = await _periodAuditActionPlanService.GetEnterpriseConfigurationByPeriodAuditId(periodAuditId);
            
            if (response.IsValid)
                return Ok(response);
            
            return BadRequest(response);
        }
    }
}
