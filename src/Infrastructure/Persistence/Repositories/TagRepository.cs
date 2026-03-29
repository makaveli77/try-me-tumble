using Microsoft.EntityFrameworkCore;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Infrastructure.Persistence.Repositories;

public class TagRepository(DataContext context) : BaseRepository<Tag>(context), ITagRepository
{
    public async Task<Tag?> GetByNameAsync(string name) =>
        await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
}
