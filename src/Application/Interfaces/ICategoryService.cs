using TryMeTumble.Application.DTOs;

namespace TryMeTumble.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto> AddCategoryAsync(CategoryDto categoryDto);
    }
}