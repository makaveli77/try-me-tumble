using TryMeTumble.Application.DTOs;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Application.Mappers
{
    public static class WebsiteMapperExtensions
    {
        public static WebsiteResponseDto ToDto(this Website website)
        {
            if (website == null) return null!;

            return new WebsiteResponseDto
            {
                Id = website.Id,
                Url = website.Url,
                Title = website.Title,
                Description = website.Description,
                Categories = website.WebsiteCategories?.Select(wc => new CategoryDto
                {
                    Name = wc.Category?.Name ?? string.Empty,
                    Description = wc.Category?.Description ?? string.Empty
                }).ToList() ?? new List<CategoryDto>(),
                Tags = website.WebsiteTags?.Select(wt => wt.Tag?.Name ?? string.Empty)
                        .Where(t => !string.IsNullOrEmpty(t)).ToList() ?? new List<string>()
            };
        }

        public static Website ToEntity(this WebsiteDto dto, Guid userId)
        {
            if (dto == null) return null!;

            var website = new Website
            {
                Id = Guid.NewGuid(),
                Url = dto.Url,
                Title = dto.Title ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                SubmittedById = userId
            };

            if (dto.CategoryId.HasValue)
            {
                website.WebsiteCategories.Add(new WebsiteCategory 
                { 
                    CategoryId = dto.CategoryId.Value,
                    WebsiteId = website.Id
                });
            }

            return website;
        }

        public static SavedWebsiteDto ToDto(this SavedWebsite savedWebsite)
        {
            if (savedWebsite == null) return null!;

            return new SavedWebsiteDto
            {
                Id = savedWebsite.Id,
                WebsiteId = savedWebsite.WebsiteId,
                Website = savedWebsite.Website?.ToDto()!,
                SavedAt = savedWebsite.CreatedAt
            };
        }

        public static UpvotedWebsiteDto ToDto(this Upvote upvote)
        {
            if (upvote == null) return null!;

            return new UpvotedWebsiteDto
            {
                Id = upvote.Id,
                WebsiteId = upvote.WebsiteId,
                Website = upvote.Website?.ToDto()!,
                UpvotedAt = upvote.CreatedAt
            };
        }
    }
}