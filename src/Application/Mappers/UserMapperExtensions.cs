using TryMeTumble.Application.DTOs;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Application.Mappers;

public static class UserMapperExtensions
{
    public static UserProfileDto ToProfileDto(this User user)
    {
        if (user == null) return null!;
        
        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin
        };
    }

    public static UserDto ToDto(this User user)
    {
        if (user == null) return null!;
        
        return new UserDto
        {
            Username = user.Username,
            Email = user.Email,
            Password = string.Empty 
        };
    }
}
