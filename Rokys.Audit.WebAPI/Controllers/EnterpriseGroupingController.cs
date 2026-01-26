using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.EnterpriseGrouping;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/enterprise-grouping")]
    public class EnterpriseGroupingController : ControllerBase
    {
        private readonly IEnterpriseGroupingService _enterpriseGroupingService;

        public EnterpriseGroupingController(IEnterpriseGroupingService enterpriseGroupingService)
        {
            _enterpriseGroupingService = enterpriseGroupingService;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetPaged([FromQuery] EnterpriseGroupingFilterRequestDto enterpriseGroupingFilterRequestDto)
        {
            var response = await _enterpriseGroupingService.GetPaged(enterpriseGroupingFilterRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _enterpriseGroupingService.GetById(id);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EnterpriseGroupingCreateRequestDto requestDto)
        {
            var response = await _enterpriseGroupingService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] EnterpriseGroupingRequestDto requestDto)
        {
            var response = await _enterpriseGroupingService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _enterpriseGroupingService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpDelete("/delete-enterprise-group/{id}")]
        public async Task<IActionResult> DeleteEnterpriseGroup([FromRoute] Guid id)
        {
            var response = await _enterpriseGroupingService.DeleteEnterpriseGroupById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
