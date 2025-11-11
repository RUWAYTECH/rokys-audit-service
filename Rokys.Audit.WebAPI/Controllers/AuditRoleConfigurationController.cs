using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.WebAPI.Attributes;

namespace Rokys.Audit.WebAPI.Controllers
{
    /// <summary>
    /// Controlador para configuraciones de roles de auditoría
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/audit-role-configurations")]
    public class AuditRoleConfigurationController : ControllerBase
    {
        private readonly IAuditRoleConfigurationService _auditRoleConfigurationService;

        public AuditRoleConfigurationController(IAuditRoleConfigurationService auditRoleConfigurationService)
        {
            _auditRoleConfigurationService = auditRoleConfigurationService;
        }

        /// <summary>
        /// Obtiene todas las configuraciones de roles activas ordenadas por secuencia
        /// </summary>
        /// <returns>Lista de configuraciones de roles activas</returns>
        [HttpGet]
        public async Task<IActionResult> GetActiveConfigurations()
        {
            var response = await _auditRoleConfigurationService.GetActiveConfigurationsAsync();
            
            if (response.IsValid)
                return Ok(response);
                
            return BadRequest(response);
        }
    }
}