using Microsoft.EntityFrameworkCore;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Infrastructure.Persistence.Repositories
{
    public class TagRepository : BaseRepository<Tag>, ITagRepository
    {
        public TagRepository(DataContext context) : base(context) { }

        public async Task<Tag?> GetByNameAsync(string name) =>
            await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
    }
}