using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokys.Audit.DTOs.Requests.Proveedor;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.WebAPI.Configuration;

namespace Rokys.Audit.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/proveedores")]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;
        public ProveedorController(IProveedorService proveedorService, IHttpContextAccessor httpContextAccessor)
        {
            _proveedorService = proveedorService;
        }

        [AllowAnonymous]
        [HttpGet("{idProveedor}")]
        public async Task<IActionResult> GetById(int idProveedor)
        {
            return Ok(await _proveedorService.GetById(idProveedor));
        }
        
        [HttpGet("getbyfilter")]
        public async Task<IActionResult> GetByFilter([FromQuery] ProveedorRequestDto request)
        {
            return Ok(await _proveedorService.Get(request));
        }

        [HttpPost()]
        public async Task<IActionResult> Create(ProveedorRequestDto requestDto)
        {           
            var response = await _proveedorService.Create(requestDto);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPut("{idProveedor}")]
        public async Task<IActionResult> Update(int idProveedor ,ProveedorRequestDto requestDto)
        {
           
            var response = await _proveedorService.Update(idProveedor, requestDto);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);

        }

        [HttpDelete("{idProveedor}")]
        public async Task<IActionResult> Delete(int idProveedor)
        {
            var response = await _proveedorService.Delete(idProveedor);

            if (response.IsValid)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("validate/{ruc}")]
        public async Task<IActionResult> GetByRuc(string ruc)
        {
            return Ok(await _proveedorService.GetByRuc(ruc));
        }
    }
}
