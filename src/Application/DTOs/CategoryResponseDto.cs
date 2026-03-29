namespace TryMeTumble.Application.DTOs;

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}
