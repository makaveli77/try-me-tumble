using Microsoft.EntityFrameworkCore;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Infrastructure.Persistence.Repositories
{
    public class WebsiteRepository : BaseRepository<Website>, IWebsiteRepository
    {
        public WebsiteRepository(DataContext context) : base(context) { }

        public async Task<Website?> GetRandomAsync(Guid? categoryId = null)
        {
            var query = _context.Websites
                .Include(w => w.WebsiteCategories).ThenInclude(wc => wc.Category)
                .Include(w => w.WebsiteTags).ThenInclude(wt => wt.Tag)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(w => w.WebsiteCategories.Any(wc => wc.CategoryId == categoryId.Value));
            }
            
            var count = await query.CountAsync();
            if (count == 0) return null;
            var index = new Random().Next(count);
            return await query.Skip(index).FirstOrDefaultAsync();
        }

        public async Task<Website?> GetByUrlAsync(string url) => 
            await _context.Websites
                .Include(w => w.WebsiteCategories).ThenInclude(wc => wc.Category)
                .Include(w => w.WebsiteTags).ThenInclude(wt => wt.Tag)
                .FirstOrDefaultAsync(w => w.Url == url);

        public async Task<IEnumerable<Website>> GetRankedWebsitesAsync()
        {
            return await _context.Websites
                .Include(w => w.WebsiteCategories).ThenInclude(wc => wc.Category)
                .Include(w => w.WebsiteTags).ThenInclude(wt => wt.Tag)
                .Include(w => w.Upvotes)
                .Include(w => w.SavedWebsites)
                .ToListAsync();
        }

        public async Task<IEnumerable<Website>> GetByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 20) =>
            await _context.WebsiteCategories
                .Where(wc => wc.CategoryId == categoryId)
                .Select(wc => wc.Website)
                .Include(w => w.WebsiteCategories).ThenInclude(wc => wc.Category)
                .Include(w => w.WebsiteTags).ThenInclude(wt => wt.Tag)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public new async Task<Website?> GetByIdAsync(Guid id) =>
            await _context.Websites
                .Include(w => w.WebsiteCategories).ThenInclude(wc => wc.Category)
                .Include(w => w.WebsiteTags).ThenInclude(wt => wt.Tag)
                .FirstOrDefaultAsync(w => w.Id == id);
    }
}