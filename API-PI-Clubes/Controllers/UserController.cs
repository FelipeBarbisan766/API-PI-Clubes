 using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.GetById(id, cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserDTO dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId(); 
            var result = await _service.Update(userId, dto, cancellationToken);
            return Ok(result);
        }
        [Authorize]
        [HttpPut("avatar")]
        public async Task<IActionResult> UpdateAvatar( UpdateAvatarDTO dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _service.UpdateAvatar(userId, dto, cancellationToken);
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _service.Delete(userId, cancellationToken);
            return NoContent();
        }
    }
}
