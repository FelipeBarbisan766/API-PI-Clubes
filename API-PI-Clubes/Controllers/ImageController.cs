using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Imagees.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _service;
        public ImageController(IImageService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string fileName)
        {
            var userId = User.GetUserId();
            var result = await _service.DeleteImageAsync(userId, fileName);
            if (!result) return NotFound();
            return Ok();
        }
        
    }
}
