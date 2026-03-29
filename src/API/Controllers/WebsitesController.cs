using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;

namespace TryMeTumble.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebsitesController(IWebsiteService websiteService, ISeederService seederService) : ControllerBase
{
    [HttpPost("seed")]
    public async Task<IActionResult> SeedBulkWebsites([FromQuery] int count = 20000)
    {
        var totalSeeded = await seederService.SeedWebsitesAsync(count);
        return Ok(new { message = $"Successfully generated and seeded {totalSeeded} randomly mocked websites to Database and Redis." });
    }

    [HttpGet("discover")]
    public async Task<ActionResult<WebsiteResponseDto>> Discover([FromQuery] Guid? categoryId)
    {
        var result = await websiteService.GetRandomWebsiteAsync(categoryId);
        return result == null ? NotFound("No websites found.") : Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<WebsiteResponseDto>> CreateWebsite(WebsiteDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await websiteService.SubmitWebsiteAsync(request, userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WebsiteResponseDto>> GetWebsite(Guid id)
    {
        var result = await websiteService.GetWebsiteByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("by-category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<WebsiteResponseDto>>> GetWebsitesByCategory(Guid categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await websiteService.GetWebsitesByCategoryAsync(categoryId, page, pageSize);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{id}/upvote")]
    public async Task<IActionResult> UpvoteWebsite(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await websiteService.UpvoteWebsiteAsync(id, userId);
        if (!success) return NotFound("Website not found");
        return Ok(new { message = "Successfully upvoted" });
    }

    [Authorize]
    [HttpPost("{id}/save")]
    public async Task<IActionResult> SaveWebsite(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await websiteService.SaveWebsiteAsync(id, userId);
        if (!success) return NotFound("Website not found");
        return Ok(new { message = "Successfully saved" });
    }

    [Authorize]
    [HttpPost("{id}/report")]
    public async Task<IActionResult> ReportWebsite(Guid id, [FromBody] ReportRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await websiteService.ReportWebsiteAsync(id, userId, request.Reason);
        if (!success) return NotFound("Website not found");
        return Ok(new { message = "Successfully reported" });
    }

    [Authorize] // Ideally an Admin policy would be here
    [HttpGet("reports")]
    public async Task<ActionResult<IEnumerable<ReportResponseDto>>> GetReports([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await websiteService.GetUnresolvedReportsAsync(page, pageSize);
        return Ok(result);
    }
}


