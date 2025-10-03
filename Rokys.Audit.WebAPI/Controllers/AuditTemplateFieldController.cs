using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditTemplateField;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/audit-template-fields")]
    public class AuditTemplateFieldController : ControllerBase
    {
        private readonly IAuditTemplateFieldService _auditTemplateFieldService;
        public AuditTemplateFieldController(IAuditTemplateFieldService auditTemplateFieldService)
        {
            _auditTemplateFieldService = auditTemplateFieldService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuditTemplateFieldRequestDto requestDto)
        {
            var response = await _auditTemplateFieldService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] AuditTemplateFieldFilterRequestDto enterpriseFilterRequestDto)
        {
            var response = await _auditTemplateFieldService.GetPaged(enterpriseFilterRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _auditTemplateFieldService.GetById(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AuditTemplateFieldRequestDto requestDto)
        {
            var response = await _auditTemplateFieldService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _auditTemplateFieldService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
