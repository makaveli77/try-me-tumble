using Microsoft.AspNetCore.Mvc;
using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;

namespace TryMeTumble.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileDto>> GetUser(Guid id)
        {
            var user = await _userService.GetUserProfileAsync(id);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        [HttpGet("{id}/saved")]
        public async Task<ActionResult<IEnumerable<SavedWebsiteDto>>> GetSavedWebsites(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var saves = await _userService.GetUserSavedWebsitesAsync(id, page, pageSize);
            return Ok(saves);
        }

        [HttpGet("{id}/upvotes")]
        public async Task<ActionResult<IEnumerable<UpvotedWebsiteDto>>> GetUpvotedWebsites(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var upvotes = await _userService.GetUserUpvotedWebsitesAsync(id, page, pageSize);
            return Ok(upvotes);
        }
    }
}