using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Domain.Interfaces
{
    public interface ISavedWebsiteRepository : IBaseRepository<SavedWebsite>
    {
        Task<IEnumerable<SavedWebsite>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<SavedWebsite?> GetByUserAndWebsiteAsync(Guid userId, Guid websiteId);
    }
}