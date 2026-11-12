using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly IRootService _service;

        public RootController(IRootService service)
        {
            _service = service;
        }

        [HttpGet("overview")]
        [Authorize(Roles = "Root")]
        public async Task<IActionResult> Overview(CancellationToken cancellationToken)
        {
            var result = await _service.Overview(cancellationToken);
            return Ok(result);
        }
    }
}