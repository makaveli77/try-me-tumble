using Microsoft.EntityFrameworkCore;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Infrastructure.Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Website> Websites { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<WebsiteCategory> WebsiteCategories { get; set; } = null!;
        public DbSet<Upvote> Upvotes { get; set; } = null!;
        public DbSet<SavedWebsite> SavedWebsites { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<WebsiteTag> WebsiteTags { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }
    }
}
