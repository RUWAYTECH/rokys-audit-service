using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Group;
using Rokys.Audit.DTOs.Responses.Group;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GroupRequestDto requestDto)
        {
            var response = await _groupService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] GroupFilterRequestDto groupFilterRequestDto)
        {
            var response = await _groupService.GetPaged(groupFilterRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _groupService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] GroupRequestDto requestDto)
        {
            var response = await _groupService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _groupService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        /// <summary>
        /// Clona un grupo y todas sus entidades hijas a una nueva empresa
        /// </summary>
        /// <param name="requestDto">Datos para la clonación del grupo</param>
        /// <returns>Información del grupo clonado y estadísticas de clonación</returns>
        [HttpPost("clone")]
        public async Task<IActionResult> Clone([FromBody] GroupCloneRequestDto requestDto)
        {
            var response = await _groupService.CloneGroupAsync(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}