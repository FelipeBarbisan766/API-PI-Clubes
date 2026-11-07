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
        private readonly IShareImageService _shareImageService;

        public ShareController(IShareService shareService, IShareImageService shareImageService)
        {
            _shareService = shareService;
            _shareImageService = shareImageService;
        }

        [HttpGet("api/share/club/{clubId:guid}")]
        public async Task<IActionResult> Club(Guid clubId, CancellationToken cancellationToken)
        {
            var html = await _shareService.BuildClubSharePageAsync(clubId, cancellationToken);
            if (html is null)
                return NotFound();

            Response.Headers.CacheControl = "public, max-age=300";
            return Content(html, "text/html; charset=utf-8");
        }

        [HttpGet("api/club/{clubId:guid}/share-image")]
        [EnableRateLimiting("share-image")]
        public async Task<IActionResult> ClubImage(Guid clubId, CancellationToken cancellationToken)
        {
            var image = await _shareImageService.GetClubImageAsync(clubId, cancellationToken);
            if (image is null)
                return NotFound();

            Response.Headers.CacheControl = "public, max-age=3600";

            return File(image.Bytes, "image/jpeg",
                lastModified: null,
                entityTag: new EntityTagHeaderValue($"\"{image.Version}\""));
        }
    }
}