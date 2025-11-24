using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;
using Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult;
using Rokys.Audit.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/period-audit-scale-result")]
    public class PeriodAuditScaleResultController : ControllerBase
    {
        private readonly IPeriodAuditScaleResultService _service;

        public PeriodAuditScaleResultController(IPeriodAuditScaleResultService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PeriodAuditScaleResultRequestDto dto)
        {
            var result = await _service.Create(dto);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePeriodAuditScaleResultRequestDto dto)
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
        public async Task<IActionResult> GetPaged([FromQuery] PeriodAuditScaleResultFilterRequestDto filter)
        {
            var result = await _service.GetPaged(filter);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("custom/{id}")]
        public async Task<IActionResult> GetByIdCustomData(Guid id)
        {
            var result = await _service.GetByIdCustomData(id);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("update-all-field-values/{periodAuditScaleResultId}")]
        public async Task<IActionResult> UpdateAllFieldValues(Guid periodAuditScaleResultId, [FromBody] PeriodAuditFieldValuesUpdateAllValuesRequestDto periodAuditFieldValuesUpdateAllValuesRequestDto)
        {
            var result = await _service.UpdateAllFieldValues(periodAuditScaleResultId, periodAuditFieldValuesUpdateAllValuesRequestDto);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Cambia el orden de un PeriodAuditScaleResult dentro de un PeriodAuditGroupResult
        /// </summary>
        /// <param name="request">Datos para el cambio de orden</param>
        /// <returns>Resultado del cambio de orden</returns>
        [HttpPut("change-order")]
        public async Task<IActionResult> ChangeOrder([FromBody] ChangePeriodAuditScaleResultOrderRequestDto request)
        {
            var response = await _service.ChangeOrder(request.PeriodAuditGroupResultId, request.CurrentPosition, request.NewPosition);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
