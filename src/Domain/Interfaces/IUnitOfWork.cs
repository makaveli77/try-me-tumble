namespace TryMeTumble.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IWebsiteRepository Websites { get; }
        ICategoryRepository Categories { get; }
        ITagRepository Tags { get; }
        IUpvoteRepository Upvotes { get; }
        ISavedWebsiteRepository SavedWebsites { get; }
        IReportRepository Reports { get; }
        Task<int> CompleteAsync();
    }
}