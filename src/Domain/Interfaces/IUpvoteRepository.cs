using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Domain.Interfaces
{
    public interface IUpvoteRepository : IBaseRepository<Upvote>
    {
        Task<IEnumerable<Upvote>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<Upvote?> GetByUserAndWebsiteAsync(Guid userId, Guid websiteId);
    }
}