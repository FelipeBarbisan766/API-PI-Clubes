using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Infrastructure.Security;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

        }
        [Authorize]
        [HttpGet("Test")]
        public IActionResult GetProtectedData()
        {
            return Ok("Você está autenticado");
        }
    }
}
