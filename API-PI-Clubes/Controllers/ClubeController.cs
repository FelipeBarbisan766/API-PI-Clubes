using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    public class ClubeController
    {
        [Route("api/clubes")]
        [ApiController]
        public class ClubesController : ControllerBase
        {
            [HttpGet]
            public IActionResult Get()
            {
                return Ok("API de Clubes funcionando!");
            }
        }
    }
}
