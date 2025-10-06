using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.MaintenanceTable;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Services.Services;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/maintenance-tables")]
    public class MaintenanceTableController : ControllerBase
    {
        private readonly IMaintenanceTableService _maintenanceTableService;
        public MaintenanceTableController(IMaintenanceTableService maintenanceTableService)
        {
            _maintenanceTableService = maintenanceTableService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] MaintenanceTableFilterRequestDto paginationRequestDto)
        {
            var response = await _maintenanceTableService.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _maintenanceTableService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MaintenanceTableRequestDto requestDto)
        {
            var response = await _maintenanceTableService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] MaintenanceTableRequestDto requestDto)
        {
            var response = await _maintenanceTableService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _maintenanceTableService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
