using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
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
        public async Task<IActionResult> Login(AuthDTO dto)
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

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyToken request)
        {
            var result = await _authService.ValidateEmailToken(request.Token);
    
            if (!result)
                return BadRequest("O link de verificação é inválido ou expirou.");
        
            return Ok("E-mail verificado com sucesso!");
        }

        [HttpPost("resend")]
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

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPassword request)
        {
            var result = await _authService.ResetPassword(request);
            if (!result)
                return BadRequest("O link de verificação é inválido ou expirou.");
            return Ok("Senha recuperada com sucesso!");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _cookieAuthService.SignOutAsync(HttpContext);
            return Ok("Logout realizado com sucesso");
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated != true)
                return Ok(new { isAuthenticated = false, user = (UserDTO?)null });

            var userId = User.GetUserId();
            var result = await _authService.GetCurrentUserInfo(userId);
            return Ok(new { isAuthenticated = true, user = result });
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
        
        [Authorize]
        [HttpPatch("complete-profile")]
        public async Task<IActionResult> CompleteProfile(CompleteProfileDTO dto)
        {
            var userId = User.GetUserId();
            await _authService.CompleteProfile(userId, dto);
            return Ok("Perfil completado com sucesso! Você já pode reservar quadras.");
        }
    }
}