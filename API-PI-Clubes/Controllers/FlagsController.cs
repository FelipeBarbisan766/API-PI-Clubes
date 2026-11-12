using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlagsController : ControllerBase
    {
        private readonly IFlagService _service;

        public FlagsController(IFlagService service)
        {
        _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Root" )]
        public async Task<ActionResult<List<FlagDTO.FlagDto>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _service.GetAll(cancellationToken);
            return Ok(response);
        }

        [HttpPost("root")]
        [Authorize(Roles = "Root")]
        public async Task<IActionResult> Create(FlagDTO.CreateFlagDTO dto , CancellationToken cancellationToken)
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
        
        [HttpPost("user/{id}")]
        [Authorize(Roles = "Admin" )]
        public async Task<ActionResult<FlagDTO.PlayerFlagDto>> FlagPlayer(
            Guid id, FlagDTO.CreatePlayerFlagRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId(); 
            var result = await _service.FlagPlayer(id, userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetByPlayer), new { id }, result);
        }

        [HttpGet("user/{id}")]
        [Authorize(Roles = "Admin" )]
        public async Task<ActionResult<List<FlagDTO.PlayerFlagDto>>> GetByPlayer(Guid id, CancellationToken cancellationToken)
        {
            var response = await _service.GetByPlayerAsync(id, cancellationToken);
            return Ok(response);
        }
    }
}