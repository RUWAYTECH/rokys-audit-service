using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.DTOs.Requests.StorageFiles;
using Microsoft.AspNetCore.Authorization;

namespace Rokys.Audit.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/storage-files")]
    public class StorageFilesController : ControllerBase
    {
        private readonly IStorageFilesService _service;

        public StorageFilesController(IStorageFilesService service)
        {
            _service = service;
        }

        [HttpGet()]
        public async Task<IActionResult> GetPaged([FromQuery] StorageFileFilterRequestDto request)
        {
            var res = await _service.GetPaged(request);
            if (!res.IsValid)
                return BadRequest(res);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var res = await _service.GetById(id);
            if (!res.IsValid)
                return BadRequest(res);
            return Ok(res);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] StorageFileRequestDto request)
        {
            var res = await _service.Create(request);
            if (!res.IsValid)
                return BadRequest(res);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            if (!res.IsValid)
                return BadRequest(res);
            return Ok(res);
        }
        [HttpGet("download-files")]
        public async Task<IActionResult> GetExcelFile([FromQuery] Guid? id, [FromQuery] Guid? entityId)
        {
            var res = await _service.GetExcelFile(id, entityId);
            if (!res.IsValid)
                return BadRequest(res);
            return Ok(res);
        }
    }
}
