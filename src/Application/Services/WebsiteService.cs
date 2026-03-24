using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;
using TryMeTumble.Application.Mappers;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace TryMeTumble.Application.Services
{
    public class WebsiteService : IWebsiteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebsiteMetadataClient _metadataClient;
        private readonly IDatabase _redis;

        public WebsiteService(IUnitOfWork unitOfWork, IWebsiteMetadataClient metadataClient, IConnectionMultiplexer redis)
        {
            _unitOfWork = unitOfWork;
            _metadataClient = metadataClient;
            _redis = redis.GetDatabase();
        }

        public async Task<WebsiteResponseDto?> GetRandomWebsiteAsync(Guid? categoryId = null)
        {
            // Redis implementation for random discovery
            string redisKey = categoryId.HasValue 
                ? $"websites_list:category:{categoryId.Value}" 
                : "websites_list";
                
            var count = await _redis.ListLengthAsync(redisKey);

            if (count > 0)
            {
                var index = new Random().Next((int)count);
                var websiteJson = await _redis.ListGetByIndexAsync(redisKey, index);
                
                if (!websiteJson.IsNullOrEmpty)
                {
                    var cachedWebsite = JsonSerializer.Deserialize<WebsiteResponseDto>(websiteJson.ToString());
                    if (cachedWebsite != null) return cachedWebsite;
                }
            }

            // Fallback to database
            var website = await _unitOfWork.Websites.GetRandomAsync(categoryId);
            if (website == null) return null;

            var dto = website.ToDto();
            
            // Cache it for future
            await _redis.ListRightPushAsync(redisKey, JsonSerializer.Serialize(dto));

            return dto;
        }

        public async Task<WebsiteResponseDto> SubmitWebsiteAsync(WebsiteDto websiteDto, Guid userId)
        {
            // 1. Uniqueness check: Does this URL already exist?
            var existingWebsite = await _unitOfWork.Websites.GetByUrlAsync(websiteDto.Url);
            if (existingWebsite != null)
            {
                return existingWebsite.ToDto();
            }

            // Map DTO to internal entity
            var website = websiteDto.ToEntity(userId);

            // Resiliently fetch third-party metadata if missing
            if (string.IsNullOrWhiteSpace(website.Title))
            {
                var meta = await _metadataClient.FetchMetadataAsync(website.Url);
                if (!string.IsNullOrWhiteSpace(meta.Title))
                {
                    website.Title = meta.Title;
                }
            }

            // Process Tags
            if (websiteDto.Tags != null && websiteDto.Tags.Any())
            {
                foreach (var tagName in websiteDto.Tags)
                {
                    var cleanName = tagName.Trim().ToLowerInvariant();
                    if (string.IsNullOrEmpty(cleanName)) continue;

                    var existingTag = await _unitOfWork.Tags.GetByNameAsync(cleanName);
                    if (existingTag == null)
                    {
                        existingTag = new Tag { Id = Guid.NewGuid(), Name = cleanName };
                        await _unitOfWork.Tags.AddAsync(existingTag);
                    }

                    website.WebsiteTags.Add(new WebsiteTag
                    {
                        WebsiteId = website.Id,
                        TagId = existingTag.Id
                    });
                }
            }

            await _unitOfWork.Websites.AddAsync(website);
            await _unitOfWork.CompleteAsync();

            var dto = website.ToDto();
            var serializedDto = JsonSerializer.Serialize(dto);
            
            // Add to Global Redis list for future discovery
            await _redis.ListRightPushAsync("websites_list", serializedDto);
            
            // Add to Category Redis list if applicable
            if (websiteDto.CategoryId.HasValue)
            {
                await _redis.ListRightPushAsync($"websites_list:category:{websiteDto.CategoryId.Value}", serializedDto);
            }
            return dto;
        }


        public async Task<bool> SaveWebsiteAsync(Guid websiteId, Guid userId)
        {
            var website = await _unitOfWork.Websites.GetByIdAsync(websiteId);
            if (website == null) return false;

            var existingSave = await _unitOfWork.SavedWebsites.GetByUserAndWebsiteAsync(userId, websiteId);
            if (existingSave != null) return true; // Already saved

            var save = new SavedWebsite
            {
                Id = Guid.NewGuid(),
                WebsiteId = websiteId,
                UserId = userId
            };

            await _unitOfWork.SavedWebsites.AddAsync(save);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> UpvoteWebsiteAsync(Guid websiteId, Guid userId)
        {
            var website = await _unitOfWork.Websites.GetByIdAsync(websiteId);
            if (website == null) return false;

            var existingUpvote = await _unitOfWork.Upvotes.GetByUserAndWebsiteAsync(userId, websiteId);
            if (existingUpvote != null) return true; // Already upvoted

            var upvote = new Upvote
            {
                Id = Guid.NewGuid(),
                WebsiteId = websiteId,
                UserId = userId
            };

            await _unitOfWork.Upvotes.AddAsync(upvote);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<WebsiteResponseDto?> GetWebsiteByIdAsync(Guid websiteId)
        {
            var website = await _unitOfWork.Websites.GetByIdAsync(websiteId);
            return website?.ToDto();
        }

        public async Task<IEnumerable<WebsiteResponseDto>> GetWebsitesByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 20)
        {
            var websites = await _unitOfWork.Websites.GetByCategoryAsync(categoryId, page, pageSize);
            return websites.Select(w => w.ToDto());
        }

        public async Task<bool> ReportWebsiteAsync(Guid websiteId, Guid userId, string reason)
        {
            var website = await _unitOfWork.Websites.GetByIdAsync(websiteId);
            if (website == null) return false;

            var report = new Report
            {
                Id = Guid.NewGuid(),
                WebsiteId = websiteId,
                UserId = userId,
                Reason = reason
            };

            await _unitOfWork.Reports.AddAsync(report);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<ReportResponseDto>> GetUnresolvedReportsAsync(int page = 1, int pageSize = 20)
        {
            var reports = await _unitOfWork.Reports.GetUnresolvedReportsAsync(page, pageSize);
            return reports.Select(r => r.ToDto());
        }
    }
}
