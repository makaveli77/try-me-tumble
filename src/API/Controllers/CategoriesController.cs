using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;

namespace TryMeTumble.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
    {
        var categories = await categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> AddCategory(CategoryDto request)
    {
        var result = await categoryService.AddCategoryAsync(request);
        return Ok(result);
    }
}
