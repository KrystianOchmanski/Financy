using Application.DTOs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var result = await _authService.RegisterAsync(registerDTO);
            if(!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }
            return Ok(new { Result = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authService.LoginAsync(loginDTO);
            if (result == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(new
            {
                AccessToken = result.Value.Token,
                RefreshToken = result.Value.RefreshToken
            });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenDTO refreshToken)
        {
            var newToken = _authService.RefreshToken(refreshToken.RefreshToken);
            if (newToken == null)
            {
                return Unauthorized("Invalid refresh token");
            }
            return Ok(new { AccessToken = newToken });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            bool result = await _authService.LogoutAsync(User);

            if (!result)
            {
                return BadRequest("Logout failed.");
            }

            return Ok("User logged out successfully.");
        }
    }
}
