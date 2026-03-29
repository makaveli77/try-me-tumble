using TryMeTumble.Domain.Interfaces;
using TryMeTumble.Infrastructure.Persistence.Repositories;

namespace TryMeTumble.Infrastructure.Persistence;

public class UnitOfWork(
    DataContext context,
    IUserRepository users,
    IWebsiteRepository websites,
    ICategoryRepository categories,
    ITagRepository tags,
    IUpvoteRepository upvotes,
    ISavedWebsiteRepository savedWebsites,
    IReportRepository reports) : IUnitOfWork
{
    public IUserRepository Users { get; } = users;
    public IWebsiteRepository Websites { get; } = websites;
    public ICategoryRepository Categories { get; } = categories;
    public ITagRepository Tags { get; } = tags;
    public IUpvoteRepository Upvotes { get; } = upvotes;
    public ISavedWebsiteRepository SavedWebsites { get; } = savedWebsites;
    public IReportRepository Reports { get; } = reports;

    public async Task<int> CompleteAsync() => await context.SaveChangesAsync();

    public void Dispose() => context.Dispose();
}
