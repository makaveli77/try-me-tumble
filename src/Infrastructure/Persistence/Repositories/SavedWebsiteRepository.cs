using Microsoft.EntityFrameworkCore;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Infrastructure.Persistence.Repositories
{
    public class SavedWebsiteRepository : BaseRepository<SavedWebsite>, ISavedWebsiteRepository
    {
        public SavedWebsiteRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<SavedWebsite>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 20) =>
            await _context.SavedWebsites
                .Include(sw => sw.Website)
                .Where(sw => sw.UserId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<SavedWebsite?> GetByUserAndWebsiteAsync(Guid userId, Guid websiteId) =>
            await _context.SavedWebsites
                .FirstOrDefaultAsync(sw => sw.UserId == userId && sw.WebsiteId == websiteId);
    }
}