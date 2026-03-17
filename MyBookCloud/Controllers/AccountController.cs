using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBookCloud.Application.Dto;
using MyBookCloud.Application.Services;

namespace MyBookCloud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService) => _authService = authService;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (token == null)
                return Unauthorized("Invalid email or password");

            return Ok(new { token });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // JWT logout is handled on the client by discarding the token.
            return Ok();
        }
    }
}
