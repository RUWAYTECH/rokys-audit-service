using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditScaleTemplate;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/audit-scale-templates")]
    public class AuditScaleTemplateController : ControllerBase
    {
        private readonly IAuditScaleTemplateService _auditScaleTemplateService;
        public AuditScaleTemplateController(IAuditScaleTemplateService auditScaleTemplateService)
        {
            _auditScaleTemplateService = auditScaleTemplateService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuditScaleTemplateRequestDto requestDto)
        {
            var response = await _auditScaleTemplateService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequestDto paginationRequestDto)
        {
            var response = await _auditScaleTemplateService.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _auditScaleTemplateService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AuditScaleTemplateRequestDto requestDto)
        {
            var response = await _auditScaleTemplateService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _auditScaleTemplateService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}