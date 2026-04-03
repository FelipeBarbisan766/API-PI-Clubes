using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Application.DTOs;
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
        [HttpPost("register")]
        public async Task<IActionResult> Register(CreatUserDTO dto)
        {
            await _authService.Register(dto);
            return Ok("Usuário registrado! Verifique seu e-mail.");
        }

        
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var result = await _authService.ValidateEmailToken(token);

            if (!result)
            {
                return BadRequest("O link de verificação é inválido ou expirou.");
            }

            return Ok("E-mail verificado com sucesso!");
        }
    }
}
