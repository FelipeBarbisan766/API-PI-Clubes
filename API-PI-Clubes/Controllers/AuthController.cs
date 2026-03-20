using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Application.Services;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController
    {
        private readonly TokenService _service;
        public AuthController(TokenService service)
        {
            _service = service;
        }

        [HttpGet("/test")]
        public async Task<IActionResult> GetToken()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Role = RoleEnum.Admin
            };
            var result = _service.Generate(user);
            return new ObjectResult(result);
        }
    }
}
