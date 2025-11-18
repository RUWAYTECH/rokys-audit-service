using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rokys.Audit.DTOs.Requests.SubstitutionHistory;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/substitution-history")]
    public class SubstitutionHistoryController : ControllerBase
    {
        private readonly ISubstitutionHistoryService _substitutionHistoryService;

        public SubstitutionHistoryController(ISubstitutionHistoryService substitutionHistoryService)
        {
            _substitutionHistoryService = substitutionHistoryService;
        }

        /// <summary>
        /// Registra una nueva suplencia de usuario en una auditoría.
        /// Valida que ambos usuarios pertenezcan al mismo rol y que la auditoría esté en estado "En proceso"
        /// </summary>
        /// <param name="requestDto">Datos de la suplencia</param>
        /// <returns>Información de la suplencia creada</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubstitutionHistoryRequestDto requestDto)
        {
            var response = await _substitutionHistoryService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        /// <summary>
        /// Lista las suplencias con paginación y filtros
        /// </summary>
        /// <param name="filterDto">Filtros de búsqueda (código de auditoría, nombres de usuarios, tienda, empresa)</param>
        /// <returns>Lista paginada de suplencias</returns>
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] SubstitutionHistoryFilterRequestDto filterDto)
        {
            var response = await _substitutionHistoryService.GetPaged(filterDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        /// <summary>
        /// Obtiene una suplencia por su ID
        /// </summary>
        /// <param name="id">ID de la suplencia</param>
        /// <returns>Información de la suplencia</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _substitutionHistoryService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
