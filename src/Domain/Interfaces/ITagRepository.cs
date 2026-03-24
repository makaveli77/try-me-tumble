using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Domain.Interfaces
{
    public interface ITagRepository : IBaseRepository<Tag>
    {
        Task<Tag?> GetByNameAsync(string name);
    }
}