using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScaleGroup;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/scale-groups")]
    public class ScaleGroupController : ControllerBase
    {
        private readonly IScaleGroupService _scaleGroupService;
        
        public ScaleGroupController(IScaleGroupService scaleGroupService)
        {
            _scaleGroupService = scaleGroupService;
        }
        
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ScaleGroupRequestDto requestDto)
        {
            var response = await _scaleGroupService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] ScaleGroupFilterRequestDto paginationRequestDto)
        {
            var response = await _scaleGroupService.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _scaleGroupService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("by-group/{groupId}")]
        public async Task<IActionResult> GetByGroupId([FromRoute] Guid groupId)
        {
            var response = await _scaleGroupService.GetByGroupId(groupId);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpPut("change-order")]
        public async Task<IActionResult> ChangeOrder([FromBody] Rokys.Audit.DTOs.Requests.ScaleGroup.ChangeScaleGroupOrderRequestDto request)
        {
            var response = await _scaleGroupService.ChangeOrder(request.GroupId, request.CurrentPosition, request.NewPosition);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] ScaleGroupRequestDto requestDto)
        {
            var response = await _scaleGroupService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _scaleGroupService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}