using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;
using TryMeTumble.Application.Mappers;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IUnitOfWork unitOfWork, IAuthService authService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserDto request)
        {
            if (await _unitOfWork.Users.GetByUsernameAsync(request.Username) != null ||
                await _unitOfWork.Users.GetByEmailAsync(request.Email) != null)
                return BadRequest("User already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _authService.HashPassword(request.Password)
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            // Decouple internal models from HTTP response
            return Ok(user.ToDto());
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
            if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
                return BadRequest("Invalid username or password.");

            user.LastLogin = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            var token = _authService.CreateToken(user);
            return Ok(token);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMe()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

            var user = await _userService.GetUserProfileAsync(userId);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Token invalidation should be handled with Redis/Blacklist in production.
            // For now, client handles deletion.
            return Ok(new { message = "Logged out successfully" });
        }
    }
}

