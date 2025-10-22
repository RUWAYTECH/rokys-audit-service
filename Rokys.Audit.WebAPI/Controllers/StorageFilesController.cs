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

        [HttpGet()]
        public async Task<IActionResult> GetPaged([FromQuery] StorageFileFilterRequestDto request)
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] StorageFileRequestDto request)
        {
            var res = await _service.Create(request);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return Ok(res);
        }
        [HttpGet("excel-file")]
        public async Task<IActionResult> GetExcelFile([FromQuery] Guid? id, [FromQuery] Guid? entityId)
        {
            var res = await _service.GetExcelFile(id, entityId);
            return Ok(res);
        }
    }
}
