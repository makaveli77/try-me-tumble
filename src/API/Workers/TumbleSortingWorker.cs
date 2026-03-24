using StackExchange.Redis;
using System.Text.Json;
using TryMeTumble.Application.Mappers;
using TryMeTumble.Domain.Interfaces;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.API.Workers
{
    public class TumbleSortingWorker : BackgroundService
    {
        private readonly ILogger<TumbleSortingWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDatabase _redis;
        private readonly int _intervalMinutes = 10; // Run every 10 mins

        public TumbleSortingWorker(
            ILogger<TumbleSortingWorker> logger,
            IServiceProvider serviceProvider,
            IConnectionMultiplexer redis)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _redis = redis.GetDatabase();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TumbleSortingWorker starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PerformSortingAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing TumbleSortingWorker.");
                }

                await Task.Delay(TimeSpan.FromMinutes(_intervalMinutes), stoppingToken);
            }
        }

        private async Task PerformSortingAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting background Tumble Sorting at: {time}", DateTimeOffset.Now);

            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var websites = await unitOfWork.Websites.GetRankedWebsitesAsync();
            var reports = await unitOfWork.Reports.GetAllAsync(); // to penalize heavily reported sites
            
            var websiteScores = websites.Select(w => 
            {
                var upvotes = w.Upvotes?.Count ?? 0;
                var saves = w.SavedWebsites?.Count ?? 0;
                var reportCount = reports.Count(r => r.WebsiteId == w.Id);

                // Scoring Algorithm
                var score = (upvotes * 2) + (saves * 3) - (reportCount * 10);
                
                // Add a small random factor to prevent stagnation
                score += new Random().Next(-2, 3);
                
                return new { Website = w, Score = score };
            })
            .Where(x => x.Score > -20) // Filter out garbage entirely
            .OrderByDescending(x => x.Score)
            .ToList();

            // Clear and rewrite Main List
            await RebuildRedisListAsync("websites_list", websiteScores.Select(x => x.Website));

            // Rewrite Category Lists
            var categories = await unitOfWork.Categories.GetAllAsync();
            foreach (var category in categories)
            {
                var catWebsites = websiteScores
                    .Where(x => x.Website.WebsiteCategories.Any(wc => wc.CategoryId == category.Id))
                    .Select(x => x.Website);
                
                await RebuildRedisListAsync($"websites_list:category:{category.Id}", catWebsites);
            }

            _logger.LogInformation("Tumble Sorting completed successfully.");
        }

        private async Task RebuildRedisListAsync(string listKey, IEnumerable<Website> websites)
        {
            // Clear old cache
            await _redis.KeyDeleteAsync(listKey);

            var items = websites
                .Select(w => JsonSerializer.Serialize(w.ToDto()))
                .Select(json => (RedisValue)json)
                .ToArray();

            if (items.Length > 0)
            {
                await _redis.ListRightPushAsync(listKey, items);
            }
        }
    }
}