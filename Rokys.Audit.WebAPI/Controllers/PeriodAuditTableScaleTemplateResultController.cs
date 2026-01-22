using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;
using Rokys.Audit.DTOs.Requests.PeriodAuditTableScaleTemplateResult;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/period-audit-table-scale-template-result")]
    [Authorize]
    public class PeriodAuditTableScaleTemplateResultController : ControllerBase
    {
        private readonly IPeriodAuditTableScaleTemplateResultService _service;

        public PeriodAuditTableScaleTemplateResultController(IPeriodAuditTableScaleTemplateResultService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost]
        [Route("filter")]
        public async Task<IActionResult> Filter([FromBody] PeriodAuditTableScaleTemplateResultFilterRequestDto filter)
        {
            var result = await _service.Filter(filter);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create([FromBody] PeriodAuditTableScaleTemplateResultRequestDto request)
        {
            var result = await _service.Create(request);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PeriodAuditTableScaleTemplateResultRequestDto request)
        {
            var result = await _service.Update(id, request);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.Delete(id);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getbyperiodauditscaleresult/{periodAuditScaleResultId}")]
        public async Task<IActionResult> GetByPeriodAuditScaleResult([FromRoute]Guid periodAuditScaleResultId)
        {
            var result = await _service.GetByPeriodAuditScaleResult(periodAuditScaleResultId);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
