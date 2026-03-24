using TryMeTumble.Domain.Interfaces;
using TryMeTumble.Infrastructure.Persistence.Repositories;

namespace TryMeTumble.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public UnitOfWork(DataContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Websites = new WebsiteRepository(_context);
            Categories = new CategoryRepository(_context);
            Tags = new TagRepository(_context);
            Upvotes = new UpvoteRepository(_context);
            SavedWebsites = new SavedWebsiteRepository(_context);
            Reports = new ReportRepository(_context);
        }

        public IUserRepository Users { get; private set; }
        public IWebsiteRepository Websites { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public ITagRepository Tags { get; private set; }
        public IUpvoteRepository Upvotes { get; private set; }
        public ISavedWebsiteRepository SavedWebsites { get; private set; }
        public IReportRepository Reports { get; private set; }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
