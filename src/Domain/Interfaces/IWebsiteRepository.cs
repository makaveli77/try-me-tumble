using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Domain.Interfaces
{
    public interface IWebsiteRepository : IBaseRepository<Website>
    {
        new Task<Website?> GetByIdAsync(Guid id);
        Task<Website?> GetRandomAsync(Guid? categoryId = null);
        Task<Website?> GetByUrlAsync(string url);
        Task<IEnumerable<Website>> GetByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 20);
        Task<IEnumerable<Website>> GetRankedWebsitesAsync();
    }
}