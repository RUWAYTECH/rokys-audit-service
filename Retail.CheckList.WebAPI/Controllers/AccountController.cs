using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.User;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.User;
using Rokys.Audit.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rokys.Audit.WebAPI.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;     


        public AccountController(JwtSettings jwtSettings, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _jwtSettings= jwtSettings;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(StatusCodeResult), 200), ProducesResponseType(typeof(ResponseDto<UserResponseDto>), 402)]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {

            var response = ResponseDto.Create<UserResponseDto>();
            var loginResult = await _userService.Login(loginRequest);
            if (loginResult.IsValid)
            {
                response.Data = new UserResponseDto();
                var userResponse = loginResult.Data;
             
                response.Data.ProfileFirstName = string.Empty;

                var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id", Guid.NewGuid().ToString()),
                        new Claim(Constants.ClaimNames.NameId, userResponse.UserName),
                        new Claim(JwtRegisteredClaimNames.Sub, userResponse.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Name, userResponse.UserName),
                        //new Claim(JwtRegisteredClaimNames.GivenName, userResponse.Usuario),
                        //new Claim(Constants.ClaimNames.Email, userResponse.Empleado.Email),
                        //new Claim(Constants.ClaimNames.EmployeeId, userResponse.Empleado.IdEmpleado.ToString()),
                        //new Claim(Constants.ClaimNames.ProfileId, userResponse.Perfiles.First().IdPerfil.ToString()),
                        //new Claim(Constants.ClaimNames.ProfileName, userResponse.Perfiles.First().Nombre),
                        //new Claim(Constants.ClaimNames.ApplicationId, userResponse.Perfiles.First().IdAplicacion.ToString()),
                        //new Claim(Constants.ClaimNames.TokenName, userResponse.Token.ToString()),
                        //new Claim(Constants.ClaimNames.VigenciaToken, userResponse.VigenciaToken.ToString()),
                        //new Claim(Constants.ClaimNames.Profiles, jsonStringPrifles),
                        //new Claim(Constants.ClaimNames.NombreCompleto, $"{userResponse.Empleado.Nombre} {userResponse.Empleado.ApellidoPaterno} {userResponse.Empleado.ApellidoMaterno}"),
                   
                     }),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinute),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                var stringToken = tokenHandler.WriteToken(token);

                response.Data.UserToken = stringToken;
                response.Data.Expiration = tokenDescriptor.Expires.Value.Subtract(DateTime.UtcNow).TotalMilliseconds;

                return Ok(response);

            }
            else
                response.Messages.AddRange(loginResult.Messages);

               
            return BadRequest(response);
        }


        [Authorize]
        [HttpGet("permissions")]
        [ProducesResponseType(typeof(StatusCodeResult), 200), ProducesResponseType(typeof(ResponseDto), 402)]
        public IActionResult Permissions()
        {
            var response = ResponseDto.Create<PermissionResponseDto[]>();

            if (response.IsValid)
                return Ok(response);
            return BadRequest(response);
        }

    }
}
