using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly ICourtService _service;
        private readonly IAuthorizationService _authorizationService;
        public CourtController(ICourtService service, IAuthorizationService authorizationService)
        {
            _service = service;
            _authorizationService = authorizationService;
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatCourtDTO dto)
        {
            var result = await _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("images/{id}")]
        public async Task<IActionResult> AddMoreImages(Guid id, [FromForm] UploadImageDTO dto)
        {
            await _service.AddMoreImagesAsync(id, dto);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateCourtDTO dto)
        {
            var result = await _service.Update(id, dto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin,Both")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var data = await _service.GetById(id);
            var clubId = data.ClubId;
            var isAuthorized = await _authorizationService
                .AuthorizeAsync(User, clubId, "AdminClubPolicy");
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            
            await _service.Delete(id);
            return NoContent();
        }
    }
}
