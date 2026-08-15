using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _service;
        private readonly IAuthorizationService _authorizationService;
        public ClubController(IClubService service, IAuthorizationService authorizationService)
        {
            _service = service;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ClubQueryDTO query)
        {
            var result = await _service.GetAll(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/{id}")]
        public async Task<IActionResult> GetAllByAdminId(Guid id)
        {
            var result = await _service.GetAllByAdminId(id);
            return Ok(result);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/dashboard")]
        public async Task<IActionResult> GetDashboard(Guid id)
        {
            var result = await _service.GetDashboard(id);
            return Ok(result);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateClubDTO dto)
        {
            var result = await _service.Create(dto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("images/{id}")]
        public async Task<IActionResult> AddMoreImages(Guid id, [FromForm] UploadImageDTO dto)
        {
            var userId = User.GetUserId(); 
            await _service.AddMoreImagesAsync(userId,id, dto);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateClubDTO dto)
        {
            var userId = User.GetUserId(); 
            var result = await _service.Update(userId,id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.GetUserId();
            await _service.Delete(userId,id);
            return NoContent();
        }
        
        
    }
}
