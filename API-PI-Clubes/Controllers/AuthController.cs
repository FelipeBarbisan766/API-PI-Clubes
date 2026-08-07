using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Security;
using API_PI_Clubes.Infrastructure.Security.Interfaces;
using API_PI_Clubes.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICookieAuthService _cookieAuthService;

        public AuthController(IAuthService authService, ICookieAuthService cookieAuthService)
        {
            _authService = authService;
            _cookieAuthService = cookieAuthService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            try
            {
                var user = await _authService.LoginAsync(dto);
                await _cookieAuthService.SignInAsync(HttpContext, user);
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
                return BadRequest("O link de verificação é inválido ou expirou.");
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
                return BadRequest("O link de verificação é inválido ou expirou.");
            return Ok("Senha recuperada com sucesso!");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _cookieAuthService.SignOutAsync(HttpContext);
            return Ok("Logout realizado com sucesso");
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await _authService.GetCurrentUserInfo(User);
            return Ok(result);
        }

        [HttpPost("google/signup")]
        public async Task<IActionResult> GoogleSignUp([FromBody] GoogleSignUpRequest request)
        {
            await _authService.GoogleSignUp(request.IdToken);
            return Ok("Usuario gerado com sucesso!");
        }

        public record GoogleSignUpRequest(string IdToken);

        [HttpPost("google/login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleSignUpRequest request)
        {
            try
            {
                var user = await _authService.GoogleLogin(request.IdToken);
                await _cookieAuthService.SignInAsync(HttpContext, user);
                return Ok("Login realizado com sucesso");
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}