using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Infrastructure.Security;
using API_PI_Clubes.Infrastructure.Settings;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IAuthService authService, IOptions<JwtSettings> options)
        {
            _authService = authService;
            _jwtSettings = options.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, //Ativar quando for usar HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(_jwtSettings.Expiration)
                };

                Response.Cookies.Append("jwt", token, cookieOptions);

                return Ok("Login realizado com sucesso");
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
        [HttpGet("resend")]
        public async Task<IActionResult> ResendEmail(string email)
        {
            await _authService.ResendEmailToken(email);
            return Ok("Verifique seu e-mail.");
        }


        [HttpPost("requestPassword")]
        public async Task<IActionResult> RequestResetPassword(string email)
        {
            await _authService.RequestResetPassword(email);
            return Ok("Requisisao realizada! Verifique seu e-mail.");
        }


        [HttpGet("resetPassword")]
        public async Task<IActionResult> ResetPassword(string token, string password)
        {
            var result = await _authService.ResetPassword(token, password);

            if (!result)
            {
                return BadRequest("O link de verificação é inválido ou expirou.");
            }

            return Ok("Senha recuperada com sucesso!");
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok("Logout realizado com sucesso");
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await _authService.GetCurrentUserInfo(User);
            return Ok(result);
        }
    }
}
