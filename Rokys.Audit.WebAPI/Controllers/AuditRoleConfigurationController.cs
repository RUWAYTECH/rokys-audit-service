using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rokys.Audit.DTOs.Requests.AuditRoleConfiguration;
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
        /// Crea una nueva configuración de rol de auditoría
        /// </summary>
        /// <param name="requestDto">Datos de la configuración de rol</param>
        /// <returns>Configuración de rol creada</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuditRoleConfigurationRequestDto requestDto)
        {
            var response = await _auditRoleConfigurationService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        /// <summary>
        /// Obtiene configuraciones de roles de auditoría paginadas
        /// </summary>
        /// <param name="filterRequestDto">Filtros de búsqueda</param>
        /// <returns>Lista paginada de configuraciones de roles</returns>
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] AuditRoleConfigurationFilterRequestDto filterRequestDto)
        {
            var response = await _auditRoleConfigurationService.GetPaged(filterRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        /// <summary>
        /// Obtiene una configuración de rol por su ID
        /// </summary>
        /// <param name="id">ID de la configuración</param>
        /// <returns>Configuración de rol</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _auditRoleConfigurationService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        /// <summary>
        /// Obtiene todas las configuraciones de roles activas ordenadas por secuencia
        /// </summary>
        /// <returns>Lista de configuraciones de roles activas</returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveConfigurations()
        {
            var response = await _auditRoleConfigurationService.GetActiveConfigurationsAsync();
            
            if (response.IsValid)
                return Ok(response);
                
            return BadRequest(response);
        }

        /// <summary>
        /// Cambia el orden de las configuraciones de rol
        /// </summary>
        /// <param name="request">Datos para cambiar el orden</param>
        /// <returns>Resultado del cambio de orden</returns>
        [HttpPut("change-order")]
        public async Task<IActionResult> ChangeOrder([FromBody] ChangeAuditRoleConfigurationOrderRequestDto request)
        {
            var response = await _auditRoleConfigurationService.ChangeOrder(request.CurrentPosition, request.NewPosition);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        /// <summary>
        /// Actualiza una configuración de rol de auditoría
        /// </summary>
        /// <param name="id">ID de la configuración a actualizar</param>
        /// <param name="requestDto">Datos actualizados</param>
        /// <returns>Configuración de rol actualizada</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AuditRoleConfigurationRequestDto requestDto)
        {
            var response = await _auditRoleConfigurationService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        /// <summary>
        /// Elimina una configuración de rol de auditoría
        /// </summary>
        /// <param name="id">ID de la configuración a eliminar</param>
        /// <returns>Resultado de la eliminación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _auditRoleConfigurationService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}