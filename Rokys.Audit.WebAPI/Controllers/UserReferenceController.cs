using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Common;
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
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequestDto paginationRequestDto)
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

        [HttpGet("by-document-number/{documentNumber}")]
        public async Task<IActionResult> GetByDocumentNumber([FromRoute] string documentNumber)
        {
            var response = await _userReferenceService.GetByDocumentNumber(documentNumber);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetByEmail([FromRoute] string email)
        {
            var response = await _userReferenceService.GetByEmail(email);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("by-role-code/{roleCode}")]
        public async Task<IActionResult> GetByRoleCode([FromRoute] string roleCode)
        {
            var response = await _userReferenceService.GetByRoleCode(roleCode);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var response = await _userReferenceService.GetActiveUsers();
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}