using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.TableScaleTemplate;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/table-scale-templates")]
    public class TableScaleTemplateController : ControllerBase
    {
        private readonly ITableScaleTemplateService _tableScaleTemplateService;
        
        public TableScaleTemplateController(ITableScaleTemplateService tableScaleTemplateService)
        {
            _tableScaleTemplateService = tableScaleTemplateService;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TableScaleTemplateRequestDto requestDto)
        {
            var response = await _tableScaleTemplateService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] TableScaleTemplateFilterRequestDto tableScaleTemplateFilterRequestDto)
        {
            var response = await _tableScaleTemplateService.GetPaged(tableScaleTemplateFilterRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _tableScaleTemplateService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("by-scale-group/{scaleGroupId}")]
        public async Task<IActionResult> GetByScaleGroupId([FromRoute] Guid scaleGroupId)
        {
            var response = await _tableScaleTemplateService.GetByScaleGroupId(scaleGroupId);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("change-order")]
        public async Task<IActionResult> ChangeOrder([FromBody] ChangeTableScaleTemplateOrderRequestDto requestDto)
        {
            var response = await _tableScaleTemplateService.ChangeOrder(requestDto.ScaleGroupId, requestDto.CurrentPosition, requestDto.NewPosition);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] TableScaleTemplateRequestDto requestDto)
        {
            var response = await _tableScaleTemplateService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _tableScaleTemplateService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}