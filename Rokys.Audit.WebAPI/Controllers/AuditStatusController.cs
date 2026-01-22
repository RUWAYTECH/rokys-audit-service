using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditStatus;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/audit-statuses")]
    public class AuditStatusController : ControllerBase
    {
        private readonly IAuditStatusService _auditStatusService;
        public AuditStatusController(IAuditStatusService auditStatusService)
        {
            _auditStatusService = auditStatusService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuditStatusRequestDto requestDto)
        {
            var response = await _auditStatusService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequestDto paginationRequestDto)
        {
            var response = await _auditStatusService.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _auditStatusService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AuditStatusRequestDto requestDto)
        {
            var response = await _auditStatusService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _auditStatusService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
