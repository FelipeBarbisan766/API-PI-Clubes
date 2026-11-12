using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportController : ControllerBase
    {
        private readonly ISportService _service;

        public SportController(ISportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _service.GetAll(cancellationToken);
            return Ok(result);
        }

        [HttpPost("root")]
        [Authorize(Roles = "Root")]
        public async Task<IActionResult> Create(CreateSportDTO dto , CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _service.Create(userId,dto,cancellationToken);
            return Ok(result);
        }
        [HttpDelete("{id}/root")]
        [Authorize(Roles = "Root")]
        public async Task<IActionResult> Delete(Guid id,CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _service.Delete(userId,id,cancellationToken);
            return Ok();
        }
    }
}