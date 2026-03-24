using Microsoft.EntityFrameworkCore;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Infrastructure.Persistence.Repositories
{
    public class UpvoteRepository : BaseRepository<Upvote>, IUpvoteRepository
    {
        public UpvoteRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Upvote>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 20) =>
            await _context.Upvotes
                .Include(u => u.Website)
                .Where(u => u.UserId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<Upvote?> GetByUserAndWebsiteAsync(Guid userId, Guid websiteId) =>
            await _context.Upvotes
                .FirstOrDefaultAsync(u => u.UserId == userId && u.WebsiteId == websiteId);
    }
}