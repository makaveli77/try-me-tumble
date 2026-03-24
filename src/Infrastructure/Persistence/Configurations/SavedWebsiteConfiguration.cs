using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Infrastructure.Persistence.Configurations
{
    public class SavedWebsiteConfiguration : IEntityTypeConfiguration<SavedWebsite>
    {
        public void Configure(EntityTypeBuilder<SavedWebsite> builder)
        {
            builder.HasOne(sw => sw.User)
                .WithMany(u => u.SavedWebsites)
                .HasForeignKey(sw => sw.UserId);
        }
    }
}