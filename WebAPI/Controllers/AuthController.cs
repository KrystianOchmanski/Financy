using Application.DTOs;
using Domain;
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

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var result = await _authService.RegisterAsync(registerDTO);
            if(!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }
            return Ok(result);
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var token = await _authService.LoginAsync(loginDTO);
            if(token == null)
            {
                return Unauthorized("Invalid email or password");
            }
            return Ok(token);
        }
    }
}
