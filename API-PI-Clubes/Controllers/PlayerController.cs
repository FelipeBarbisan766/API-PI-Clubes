using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
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
            var userId = User.GetUserId(); 
            var result = await _service.GetCurrentUserInfo(userId);
            return Ok(result);
        }
        
        // [Authorize]
        // [HttpPut("{id}")]
        // public async Task<IActionResult> Update(Guid id, UpdatePlayerDTO dto)
        // {
        //     var userId = User.GetUserId();
        //     var result = await _service.Update(userId, id, dto);
        //     return Ok(result);
        // }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.GetUserId();
            await _service.Delete(userId, id);
            return NoContent();
        }
        [HttpGet("name/{profileName}")]
        public async Task<IActionResult> GetByProfileName(string profileName)
        {
            var result = await _service.GetByProfileName(profileName);
            return Ok(result);
        }

        [HttpPost("{id}/profile-name")]
        [Authorize]
        public async Task<IActionResult> SetProfileName(Guid id, [FromBody] SetProfileNameDTO dto)
        {
            var userId = User.GetUserId();
            var result = await _service.SetProfileName(userId, id, dto);
            return Ok(result);
        }
        [HttpGet("{id}/favorite-sports")]
        public async Task<IActionResult> GetFavoriteSports(Guid id)
        {
            var result = await _service.GetFavoriteSports(id);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id}/favorite-sports")]
        public async Task<IActionResult> AddFavoriteSports(Guid id, [FromBody] AddFavoriteSportsDTO dto)
        {
            var userId = User.GetUserId();
            var result = await _service.AddFavoriteSports(userId, id, dto);
            return Ok(result);
        }
        [Authorize]
        [HttpPut("{id}/favorite-sports")]
        public async Task<IActionResult> SetFavoriteSports(Guid id, [FromBody] SetFavoriteSportsDTO dto)
        {
            var userId = User.GetUserId();
            var result = await _service.SetFavoriteSports(userId, id, dto);
            return Ok(result);
        }
    }
}
