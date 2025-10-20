using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.DTOs.Requests.StorageFiles;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/storage-files")]
    public class StorageFilesController : ControllerBase
    {
        private readonly IStorageFilesService _service;

        public StorageFilesController(IStorageFilesService service)
        {
            _service = service;
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] StorageFileFilterRequestDto request)
        {
            var res = await _service.GetPaged(request);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var res = await _service.GetById(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StorageFileRequestDto request)
        {
            var res = await _service.Create(request);
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] StorageFileRequestDto request)
        {
            var res = await _service.Update(id, request);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return Ok(res);
        }
    }
}
