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
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            if (result.Data == null) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [Route("filter")]
        public async Task<IActionResult> Filter([FromBody] PeriodAuditTableScaleTemplateResultFilterRequestDto filter)
        {
            var result = await _service.Filter(filter);
            return Ok(result);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create([FromBody] PeriodAuditTableScaleTemplateResultRequestDto request)
        {
            var result = await _service.Create(request);
            return Ok(result);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PeriodAuditTableScaleTemplateResultRequestDto request)
        {
            var result = await _service.Update(id, request);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.Delete(id);
            if (!result.Data) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("getbyperiodauditscaleresult/{periodAuditScaleResultId}")]
        public async Task<IActionResult> GetByPeriodAuditScaleResult([FromRoute]Guid periodAuditScaleResultId)
        {
            var result = await _service.GetByPeriodAuditScaleResult(periodAuditScaleResultId);
            if (result.Data == null) return NotFound(result);
            return Ok(result);
        }

        [HttpPut("updatebyperiodauditsacaleresult/{id}")]
        public async Task<IActionResult> UpdateAllFieldValues([FromRoute]Guid id, [FromBody] PeriodAuditFieldValuesUpdateAllValuesRequestDto request)
        {
            var result = await _service.UpdateAllFieldValues(id, request);
            return Ok(result);
        }
    }
}
