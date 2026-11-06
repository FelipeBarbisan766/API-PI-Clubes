using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Net.Http.Headers;

namespace API_PI_Clubes.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class ShareController : ControllerBase
    {
        private readonly IShareService _shareService;

        public ShareController(IShareService shareService)
        {
            _shareService = shareService;
        }

        [HttpGet("api/club/{clubId:guid}/share-image")]
        [EnableRateLimiting("share-image")]
        public async Task<IActionResult> ClubImage(Guid clubId, CancellationToken cancellationToken)
        {
            var image = await _shareService.GetClubImageAsync(clubId, cancellationToken);
            if (image is null)
                return NotFound();

            // max-age curto + ETag: se preço/capa mudarem, o hash muda e o front recebe a imagem nova.
            Response.Headers.CacheControl = "public, max-age=300";

            return File(
                image.Bytes,
                "image/jpeg",
                lastModified: null,
                entityTag: new EntityTagHeaderValue($"\"{image.Version}\""));
        }
    }
}