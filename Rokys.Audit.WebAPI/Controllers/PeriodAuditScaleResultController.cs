using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Update(Guid id, [FromBody] PeriodAuditScaleResultRequestDto dto)
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
    }
}
