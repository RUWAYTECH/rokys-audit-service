using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScaleCompany;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/scale-companies")]
    public class ScaleCompanyController : ControllerBase
    {
        private readonly IScaleCompanyService _scaleCompanyService;
        public ScaleCompanyController(IScaleCompanyService scaleCompanyService)
        {
            _scaleCompanyService = scaleCompanyService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScaleCompanyRequestDto requestDto)
        {
            var response = await _scaleCompanyService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequestDto paginationRequestDto)
        {
            var response = await _scaleCompanyService.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _scaleCompanyService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ScaleCompanyRequestDto requestDto)
        {
            var response = await _scaleCompanyService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _scaleCompanyService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
