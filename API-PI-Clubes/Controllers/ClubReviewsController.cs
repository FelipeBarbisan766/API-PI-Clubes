using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers;
[ApiController]
[Route("api/clubs/{clubId:guid}/reviews")]
[Authorize]
public class ClubReviewsController : ControllerBase
{
    private readonly IClubReviewService _service;

    public ClubReviewsController(IClubReviewService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> RateClub(Guid clubId, [FromBody] CreateClubReviewDTO dto)
    {
        var userId = User.GetUserId();
        var summary = await _service.RateClub(userId, clubId, dto);
        return Ok(summary);
    }

    [HttpGet("summary")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSummary(Guid clubId)
    {
        var summary = await _service.GetSummary(clubId);
        return Ok(summary);
    }
}