using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.Store;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/stores")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;
        public StoreController(IStoreService storeService, IHttpContextAccessor httpContextAccessor)
        {
            _storeService = storeService;
        }

        [AllowAnonymous]
        [HttpGet("{idStore}")]
        public async Task<IActionResult> GetById(Guid idStore)
        {
            return Ok(await _storeService.GetById(idStore));
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPaged([FromQuery] StoreFilterRequestDto filterDto)
        {
            var result = await _storeService.GetPaged(filterDto);
            if (result.IsValid)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(StoreRequestDto requestDto)
        {
            var response = await _storeService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
