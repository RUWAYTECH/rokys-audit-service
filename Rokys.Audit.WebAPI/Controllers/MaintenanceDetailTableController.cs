using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.MaintenanceDetailTable;
using Rokys.Audit.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/maintenancedetailtable")]
    public class MaintenanceDetailTableController : ControllerBase
    {
        private readonly IMaintenanceDetailTableService _service;

        public MaintenanceDetailTableController(IMaintenanceDetailTableService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MaintenanceDetailTableRequestDto requestDto)
        {
            var response = await _service.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] MaintenanceDetailTableFilterRequestDto paginationRequestDto)
        {
            var response = await _service.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _service.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] MaintenanceDetailTableRequestDto requestDto)
        {
            var response = await _service.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _service.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
