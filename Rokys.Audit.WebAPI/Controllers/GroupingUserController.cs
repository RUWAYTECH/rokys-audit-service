using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.GroupingUser;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/grouping-users")]
    public class GroupingUserController : ControllerBase
    {
        private readonly IGroupingUserService _groupingUserService;
        public GroupingUserController(IGroupingUserService groupingUserService)
        {
            _groupingUserService = groupingUserService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GroupingUserUpsertRequestDto requestDto)
        {
            var response = await _groupingUserService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] GroupingUserFilterRequestDto filterDto)
        {
            var response = await _groupingUserService.GetPaged(filterDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _groupingUserService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] GroupingUserRequestDto requestDto)
        {
            var response = await _groupingUserService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _groupingUserService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
