using API_PI_Clubes.Application.Interfaces.IServices;
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
    }
}