using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers;
[ApiController]
[Route("api/club/{clubId:guid}/reviews")]
[Authorize]
public class ClubReviewsController : ControllerBase
{
    private readonly IClubReviewService _service;

    public ClubReviewsController(IClubReviewService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> RateClub(Guid clubId, [FromBody] CreateClubReviewDTO dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var summary = await _service.RateClub(userId, clubId, dto, cancellationToken);
        return Ok(summary);
    }

    [HttpGet("summary")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSummary(Guid clubId, CancellationToken cancellationToken)
    {
        var summary = await _service.GetSummary(clubId, cancellationToken);
        return Ok(summary);
    }
    [HttpGet("verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyReview(Guid clubId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var summary = await _service.VerifyReview(userId, clubId, cancellationToken);
        return Ok(summary);
    }
}