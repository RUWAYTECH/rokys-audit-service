using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScoringCriteria;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/scoring-criterias")]
    public class ScoringCriteriaController : ControllerBase
    { 
        private readonly IScoringCriteriaService _scoringCriteriaService;
        public ScoringCriteriaController(IScoringCriteriaService scoringCriteriaService)
        {
            _scoringCriteriaService = scoringCriteriaService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScoringCriteriaRequestDto requestDto)
        {
            var response = await _scoringCriteriaService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequestDto filterRequestDto)
        {
            var response = await _scoringCriteriaService.GetPaged(filterRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _scoringCriteriaService.GetById(id);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ScoringCriteriaRequestDto requestDto)
        {
            var response = await _scoringCriteriaService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _scoringCriteriaService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
