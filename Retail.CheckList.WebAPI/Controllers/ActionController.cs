using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionController : ControllerBase
    {
        IActionService _actionService;
        public ActionController(IActionService actionService)
        {
            _actionService = actionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _actionService.GetAsync();
            if (response.IsValid)
                return Ok(response);
            return BadRequest();
        }
    }
}
