using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/period-audit-group-result")]
    public class PeriodAuditGroupResultController : ControllerBase
    {
        private readonly IPeriodAuditGroupResultService _service;

        public PeriodAuditGroupResultController(IPeriodAuditGroupResultService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PeriodAuditGroupResultRequestDto dto)
        {
            var result = await _service.Create(dto);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePeriodAuditGroupResultRequestDto dto)
        {
            var result = await _service.Update(id, dto);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.Delete(id);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PeriodAuditGroupResultFilterRequestDto paginationRequestDto)
        {
            var response = await _service.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("recalculate/{periodAuditGroupResultId}")]
        public async Task<IActionResult> Recalculate([FromRoute]Guid periodAuditGroupResultId)
        {
            var response = await _service.Recalculate(periodAuditGroupResultId);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        /// <summary>
        /// Cambia el orden de un PeriodAuditGroupResult dentro de un PeriodAudit
        /// </summary>
        /// <param name="request">Datos para el cambio de orden</param>
        /// <returns>Resultado del cambio de orden</returns>
        [HttpPut("change-order")]
        public async Task<IActionResult> ChangeOrder([FromBody] ChangePeriodAuditGroupResultOrderRequestDto request)
        {
            var response = await _service.ChangeOrder(request.PeriodAuditId, request.CurrentPosition, request.NewPosition);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
