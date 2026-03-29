using Microsoft.EntityFrameworkCore;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Infrastructure.Persistence.Repositories;

public class CategoryRepository(DataContext context) : BaseRepository<Category>(context), ICategoryRepository
{
    public async Task<Category?> GetByNameAsync(string name) => 
        await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
}
