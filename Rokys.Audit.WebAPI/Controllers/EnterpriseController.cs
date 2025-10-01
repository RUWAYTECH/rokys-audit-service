using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.Enterprise;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/enterprise")]
    public class EnterpriseController : ControllerBase
    {
        private readonly IEnterpriseService _enterpriseService;
        public EnterpriseController(IEnterpriseService enterpriseService)
        {
            _enterpriseService = enterpriseService;
        }
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] EnterpriseFilterRequestDto enterpriseFilterRequestDto)
        {
            var response = await _enterpriseService.GetPaged(enterpriseFilterRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _enterpriseService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EnterpriseRequestDto requestDto)
        {
            var response = await _enterpriseService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
