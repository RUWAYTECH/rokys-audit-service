using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.InboxItems;
using Rokys.Audit.DTOs.Responses.InboxItems;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/inbox-items")]
    public class InboxItemsController : ControllerBase
    {
        private readonly IInboxItemsService _inboxService;
        public InboxItemsController(IInboxItemsService inboxService)
        {
            _inboxService = inboxService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InboxItemRequestDto requestDto)
        {
            var response = await _inboxService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] InboxItemFilterRequestDto filterRequestDto)
        {
            var response = await _inboxService.GetPaged(filterRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _inboxService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] InboxItemRequestDto requestDto)
        {
            var response = await _inboxService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _inboxService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
