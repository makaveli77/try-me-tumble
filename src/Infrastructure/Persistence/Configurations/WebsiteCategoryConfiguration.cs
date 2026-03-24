using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Infrastructure.Persistence.Configurations
{
    public class WebsiteCategoryConfiguration : IEntityTypeConfiguration<WebsiteCategory>
    {
        public void Configure(EntityTypeBuilder<WebsiteCategory> builder)
        {
            builder.HasKey(wc => new { wc.WebsiteId, wc.CategoryId });
        }
    }
}