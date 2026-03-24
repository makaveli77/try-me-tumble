using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Infrastructure.Persistence.Configurations
{
    public class WebsiteTagConfiguration : IEntityTypeConfiguration<WebsiteTag>
    {
        public void Configure(EntityTypeBuilder<WebsiteTag> builder)
        {
            builder.HasKey(wt => new { wt.WebsiteId, wt.TagId });
        }
    }
}