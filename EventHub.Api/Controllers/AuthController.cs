using EventHub.Api.Services;
using EventHub.Shared.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace EventHub.Api.Controllers
{

    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")] //POST api/auth/register
        public async Task<ApiResult> Register([FromBody] RegisterDto registerDto) =>
            await _authService.RegisterAsync(registerDto);


        [HttpPost("login")] //POST api/auth/login
        public async Task<ApiResult<AuthDto>> Login([FromBody] LoginDto loginDto) =>
            await _authService.LoginAsync(loginDto);

    }
}
