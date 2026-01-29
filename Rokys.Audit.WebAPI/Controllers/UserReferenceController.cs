using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Group;
using Rokys.Audit.DTOs.Requests.UserReference;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/user-references")]
    public class UserReferenceController : ControllerBase
    {
        private readonly IUserReferenceService _userReferenceService;

        public UserReferenceController(IUserReferenceService userReferenceService)
        {
            _userReferenceService = userReferenceService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserReferenceRequestDto requestDto)
        {
            var response = await _userReferenceService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] UseReferenceFilterRequestDto paginationRequestDto)
        {
            var response = await _userReferenceService.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _userReferenceService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UserReferenceRequestDto requestDto)
        {
            var response = await _userReferenceService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _userReferenceService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("by-user-id/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
        {
            var response = await _userReferenceService.GetByUserId(userId);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("by-employee-id/{employeeId}")]
        public async Task<IActionResult> GetByEmployeeId([FromRoute] Guid employeeId)
        {
            var response = await _userReferenceService.GetByEmployeeId(employeeId);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("by-enterprise-grouping-id/{enterpriseGroupingId}")]
        public async Task<IActionResult> GetUsersByEnterpriseGroupingId([FromRoute] Guid enterpriseGroupingId)
        {
            var response = await _userReferenceService.GetUsersByEnterpriseGroupingId(enterpriseGroupingId);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}