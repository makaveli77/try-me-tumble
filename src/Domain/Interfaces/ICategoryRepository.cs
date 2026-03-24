using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Domain.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
    }
}