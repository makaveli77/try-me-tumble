using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;
using TryMeTumble.Application.Mappers;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Application.Services;

public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var categories = await unitOfWork.Categories.GetAllAsync();
        return categories.Select(c => c.ToDto());
    }

    public async Task<CategoryResponseDto> AddCategoryAsync(CategoryDto categoryDto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = categoryDto.Name,
            Description = categoryDto.Description
        };

        await unitOfWork.Categories.AddAsync(category);
        await unitOfWork.CompleteAsync();

        return category.ToDto();
    }
}
