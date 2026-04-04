using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;
using TryMeTumble.Application.Mappers;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IUnitOfWork unitOfWork, IAuthService authService, IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(UserDto request)
    {
        if (await unitOfWork.Users.GetByUsernameAsync(request.Username) != null ||
            await unitOfWork.Users.GetByEmailAsync(request.Email) != null)
            return BadRequest("User already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = authService.HashPassword(request.Password)
        };

        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.CompleteAsync();

        // Decouple internal models from HTTP response
        return Ok(user.ToDto());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var user = await unitOfWork.Users.GetByUsernameAsync(request.Username);
        if (user == null || !authService.VerifyPassword(request.Password, user.PasswordHash))
            return BadRequest("Invalid username or password.");

        user.LastLogin = DateTime.UtcNow;
        await unitOfWork.Users.UpdateAsync(user);
        await unitOfWork.CompleteAsync();

        var token = authService.CreateToken(user);
        
        // Secure the token in an HttpOnly cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Force HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("jwt", token, cookieOptions);

        return Ok(new { message = "Logged in successfully", role = user.Role });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMe()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var user = await userService.GetUserProfileAsync(userId);
        if (user == null) return NotFound();

        return Ok(user);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Clear HttpOnly cookie
        Response.Cookies.Delete("jwt");
        return Ok(new { message = "Logged out successfully" });
    }
}


