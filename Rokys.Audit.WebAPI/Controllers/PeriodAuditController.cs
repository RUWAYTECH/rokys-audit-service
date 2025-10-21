using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.PeriodAudit;
using Rokys.Audit.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/periodaudit")]
    public class PeriodAuditController : ControllerBase
    {
        private readonly IPeriodAuditService _service;

        public PeriodAuditController(IPeriodAuditService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PeriodAuditRequestDto requestDto)
        {
            var response = await _service.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PeriodAuditFilterRequestDto paginationRequestDto)
        {
            var response = await _service.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _service.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] PeriodAuditRequestDto requestDto)
        {
            var response = await _service.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _service.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("last-audit-by-store/{storeId}")]
        public async Task<IActionResult> GetLastAuditByStoreId([FromRoute] Guid storeId)
        {
            var response = await _service.GetLasAuditByStoreId(storeId);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
