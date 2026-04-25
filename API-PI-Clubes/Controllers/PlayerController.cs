using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Security.Interfaces;
using API_PI_Clubes.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _service;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        public PlayerController(IPlayerService service, JwtSettings jwtSettings, IUserRepository userRepository, ITokenService tokenService)
        {
            _service = service;
            _jwtSettings = jwtSettings;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,None")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatPlayerDTO dto)
        {
            var result = await _service.Create(dto);

            var user = await _userRepository.GetByIdAsync(dto.UserId);
            var token = _tokenService.GenerateToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.Expiration)
            };

            Response.Cookies.Append("jwt", token, cookieOptions);
            return Ok(result);
        }
        
        [Authorize(Roles = "Player,Both")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdatePlayerDTO dto)
        {
            var result = await _service.Update(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Player,Both")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.Delete(id);
            return NoContent();
        }
    }
}
