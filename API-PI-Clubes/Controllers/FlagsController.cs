using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class FlagsController : ControllerBase
    {
        private readonly IFlagService _service;

        public FlagsController(IFlagService service)
        {
        _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<FlagDTO.FlagDto>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _service.GetAll(cancellationToken);
            return Ok(response);
        }

        [HttpPost("user/{id}")]
        public async Task<ActionResult<FlagDTO.PlayerFlagDto>> Create(
            Guid id, FlagDTO.CreatePlayerFlagRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId(); 
            var result = await _service.Create(id, userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetByPlayer), new { id }, result);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<List<FlagDTO.PlayerFlagDto>>> GetByPlayer(Guid id, CancellationToken cancellationToken)
        {
            var response = await _service.GetByPlayerAsync(id, cancellationToken);
            return Ok(response);
        }
    }
}