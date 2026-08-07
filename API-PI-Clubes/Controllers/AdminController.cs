using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Security.Interfaces;
using API_PI_Clubes.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;
        private readonly ICookieAuthService _cookieAuthService;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AdminController(IAdminService service,ICookieAuthService cookieAuthService, ITokenService tokenService, IUserRepository userRepository)
        {
            _service = service;
            _cookieAuthService = cookieAuthService;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }
      

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await _service.GetCurrentUserInfo(User);
            return Ok(result);
        }
    
        [Authorize(Roles = "Player")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatAdminDTO dto)
        {
            var result = await _service.Create(dto);

            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found.");

            await _cookieAuthService.SignInAsync(HttpContext, user);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateAdminDTO dto)
        {
            var result = await _service.Update(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.Delete(id);
            return NoContent();
        }
    }
}
