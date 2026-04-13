using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _service;
        public ClubController(IClubService service)
        {
            _service = service;
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
        public async Task<IActionResult> Create([FromForm] CreateClubDTO dto)
        {
            var result = await _service.Create(dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateClubDTO dto)
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
        [Authorize(Roles = "Admin")]
        [HttpPut("Image/{id}")]
        public async Task<IActionResult> Upload(Guid id ,[FromForm] UploadImageDTO dto)
        {
            var result = await _service.Upload(dto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Images/{id}")]
        public async Task<IActionResult> DeleteImages(Guid id ,DeleteImageDto dto)
        {
            await _service.DeleteImages(id, dto);
            return NoContent();
        }
    }
}
