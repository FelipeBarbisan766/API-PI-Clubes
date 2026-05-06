using API_PI_Clubes.Application.Interfaces.IServices;
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

        [Authorize(Roles = "Admin,Both")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string blobName)
        {
            var result = await _service.DeleteImageAsync(blobName);
            if (!result) return NotFound();
            return Ok();
        }
        
    }
}
