using TryMeTumble.Application.DTOs;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Application.Mappers;

public static class CategoryMapperExtensions
{
    public static CategoryResponseDto ToDto(this Category category)
    {
        if (category == null) return null!;

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}
