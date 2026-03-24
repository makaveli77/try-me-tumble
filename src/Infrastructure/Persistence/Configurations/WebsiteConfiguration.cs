using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Infrastructure.Persistence.Configurations
{
    public class WebsiteConfiguration : IEntityTypeConfiguration<Website>
    {
        public void Configure(EntityTypeBuilder<Website> builder)
        {
            builder.HasIndex(w => w.Url).IsUnique();
            
            builder.HasOne(w => w.SubmittedBy)
                .WithMany(u => u.SubmittedWebsites)
                .HasForeignKey(w => w.SubmittedById);
        }
    }
}