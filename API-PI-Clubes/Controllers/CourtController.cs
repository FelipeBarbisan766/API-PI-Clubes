using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Infrastructure.Extensions;
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
        public async Task<IActionResult> GetAll([FromQuery] CourtQueryDTO query, CancellationToken cancellationToken)
        {
            var result = await _service.GetAll(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.GetById(id, cancellationToken);
            return Ok(result);
        }
        
        [HttpGet("club/{id}")]
        public async Task<IActionResult> GetByClubId(Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByClubId(id, cancellationToken);
            return Ok(result);
        }
                
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatCourtDTO dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _service.Create(userId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("{Id}/images/")]
        public async Task<IActionResult> AddMoreImages(Guid Id, [FromForm] UploadImageDTO dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId(); 
            await _service.AddMoreImagesAsync(userId, Id, dto, cancellationToken);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{Id}/images/reorder")]
        public async Task<IActionResult> ReorderImages(Guid Id, [FromBody] ReorderImagesRequestDTO dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _service.ReorderImagesAsync(userId, Id, dto.Orders, cancellationToken);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}/images/{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid Id, Guid imageId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _service.DeleteImageAsync(userId, Id, imageId, cancellationToken);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateCourtDTO dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId(); 
            var result = await _service.Update(userId, id, dto, cancellationToken);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId(); 
            await _service.Delete(userId, id, cancellationToken);
            return NoContent();
        }
    }
}
