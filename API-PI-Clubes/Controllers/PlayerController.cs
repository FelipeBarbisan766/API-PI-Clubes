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
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.GetById(id, cancellationToken);
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId(); 
            var result = await _service.GetCurrentUserInfo(userId, cancellationToken);
            return Ok(result);
        }
        
        // [Authorize]
        // [HttpPut("{id}")]
        // public async Task<IActionResult> Update(Guid id, UpdatePlayerDTO dto, CancellationToken cancellationToken)
        // {
        //     var userId = User.GetUserId();
        //     var result = await _service.Update(userId, id, dto, cancellationToken);
        //     return Ok(result);
        // }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _service.Delete(userId, cancellationToken);
            return NoContent();
        }
        [HttpGet("name/{profileName}")]
        public async Task<IActionResult> GetByProfileName(string profileName, CancellationToken cancellationToken)
        {
            var result = await _service.GetByProfileName(profileName, cancellationToken);
            return Ok(result);
        }

        [HttpPost("profile-name")]
        [Authorize]
        public async Task<IActionResult> SetProfileName([FromBody] SetProfileNameDTO dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _service.SetProfileName(userId, dto, cancellationToken);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("favorite-sports")]
        public async Task<IActionResult> GetFavoriteSports(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _service.GetFavoriteSports(userId, cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("favorite-sports")]
        public async Task<IActionResult> AddFavoriteSports([FromBody] AddFavoriteSportsDTO dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _service.AddFavoriteSports(userId, dto, cancellationToken);
            return Ok(result);
        }
        [Authorize]
        [HttpPut("favorite-sports")]
        public async Task<IActionResult> SetFavoriteSports([FromBody] SetFavoriteSportsDTO dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _service.SetFavoriteSports(userId, dto, cancellationToken);
            return Ok(result);
        }
    }
}
