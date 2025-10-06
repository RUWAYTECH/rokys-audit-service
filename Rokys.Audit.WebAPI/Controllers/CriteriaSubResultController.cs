using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.CriteriaSubResult;
using Rokys.Audit.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/criteriasubresult")]
    public class CriteriaSubResultController : ControllerBase
    {
        private readonly ICriteriaSubResultService _criteriaSubResultService;

        public CriteriaSubResultController(ICriteriaSubResultService criteriaSubResultService)
        {
            _criteriaSubResultService = criteriaSubResultService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CriteriaSubResultRequestDto requestDto)
        {
            var response = await _criteriaSubResultService.Create(requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequestDto paginationRequestDto)
        {
            var response = await _criteriaSubResultService.GetPaged(paginationRequestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _criteriaSubResultService.GetById(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CriteriaSubResultRequestDto requestDto)
        {
            var response = await _criteriaSubResultService.Update(id, requestDto);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _criteriaSubResultService.Delete(id);
            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }
    }
}