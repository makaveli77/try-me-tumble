using Microsoft.AspNetCore.Mvc;
using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;

namespace TryMeTumble.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfileDto>> GetUser(Guid id)
    {
        var user = await userService.GetUserProfileAsync(id);
        if (user == null) return NotFound("User not found");
        return Ok(user);
    }

    [HttpGet("{id}/saved")]
    public async Task<ActionResult<IEnumerable<SavedWebsiteDto>>> GetSavedWebsites(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var saves = await userService.GetUserSavedWebsitesAsync(id, page, pageSize);
        return Ok(saves);
    }

    [HttpGet("{id}/upvotes")]
    public async Task<ActionResult<IEnumerable<UpvotedWebsiteDto>>> GetUpvotedWebsites(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var upvotes = await userService.GetUserUpvotedWebsitesAsync(id, page, pageSize);
        return Ok(upvotes);
    }
}
