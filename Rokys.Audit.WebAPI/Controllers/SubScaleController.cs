using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.SubScale;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/sub-scales")]
    public class SubScaleController : ControllerBase
    {
        private readonly ISubScaleService _subScaleService;
        public SubScaleController(ISubScaleService subScaleService)
        {
            _subScaleService = subScaleService;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubScaleRequestDto requestDto)
        {
            var response = await _subScaleService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] SubScaleFilterRequestDto paginationRequestDto)
        {
            var response = await _subScaleService.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _subScaleService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] SubScaleRequestDto requestDto)
        {
            var response = await _subScaleService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _subScaleService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
