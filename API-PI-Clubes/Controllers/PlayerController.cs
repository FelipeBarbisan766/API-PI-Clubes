using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Security.Interfaces;
using API_PI_Clubes.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
        public PlayerController(IPlayerService service, IOptions<JwtSettings> options, IUserRepository userRepository, ITokenService tokenService)
        {
            _service = service;
            _jwtSettings = options.Value;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await _service.GetCurrentUserInfo(User);
            return Ok(result);
        }
        
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdatePlayerDTO dto)
        {
            var result = await _service.Update(id, dto);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.Delete(id);
            return NoContent();
        }
    }
}
