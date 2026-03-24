using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Infrastructure.Persistence.Configurations
{
    public class UpvoteConfiguration : IEntityTypeConfiguration<Upvote>
    {
        public void Configure(EntityTypeBuilder<Upvote> builder)
        {
            builder.HasOne(uv => uv.User)
                .WithMany(u => u.Upvotes)
                .HasForeignKey(uv => uv.UserId);
        }
    }
}