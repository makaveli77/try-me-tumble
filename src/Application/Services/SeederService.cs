using Bogus;
using StackExchange.Redis;
using System.Text.Json;
using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;
using TryMeTumble.Application.Mappers;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Application.Services;

public class SeederService(IUnitOfWork unitOfWork, IConnectionMultiplexer redis) : ISeederService
{
    private readonly IDatabase _redis = redis.GetDatabase();

    public async Task<int> SeedWebsitesAsync(int count)
    {
        var systemUser = await unitOfWork.Users.GetByUsernameAsync("system_seeder");
        if (systemUser == null)
        {
            systemUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "system_seeder",
                Email = "seeder@trymetumble.com",
                PasswordHash = "NO_LOGIN_ALLOWED",
                CreatedAt = DateTime.UtcNow
            };
            await unitOfWork.Users.AddAsync(systemUser);
            await unitOfWork.CompleteAsync();
        }

        var categoryNames = new[] { "Technology", "Science", "Art", "Gaming", "Music", "Education", "News", "Entertainment" };
        var existingCategories = await unitOfWork.Categories.GetAllAsync();
        var categoriesList = existingCategories.ToList();

        if (!categoriesList.Any())
        {
            foreach (var name in categoryNames)
            {
                var cat = new Category { Id = Guid.NewGuid(), Name = name, Description = $"{name} websites" };
                await unitOfWork.Categories.AddAsync(cat);
                categoriesList.Add(cat);
            }
            await unitOfWork.CompleteAsync();
        }

        var random = new Random();

        var realWebsites = new List<(string Url, string Title, string Description, string Category)>
        {
            ("https://en.wikipedia.org/wiki/Special:Random", "Wikipedia", "Explore the world's most extensive collaborative encyclopedia by jumping into a random topic.", "Education"),
            ("https://archive.org", "The Internet Archive", "A non-profit library of millions of free books, movies, software, music, websites, and more.", "Technology"),
            ("https://news.ycombinator.com/", "Hacker News", "A social news website focusing on computer science and entrepreneurship. Runs fast and embeds well.", "Technology"),
            ("https://www.bing.com", "Bing", "Microsoft's search engine. Fully allows embedding for testing purposes.", "Technology"),
            ("https://maps.google.com/maps?q=London&t=&z=13&ie=UTF8&iwloc=&output=embed", "Google Maps", "Embedded interactive map of London.", "Education"),
            ("https://xkcd.com/", "XKCD", "A webcomic of romance, sarcasm, math, and language.", "Entertainment"),
            ("https://lobste.rs/", "Lobsters", "Computing-focused community centered around link aggregation and discussion.", "Technology"),
            ("https://w3c.github.io/html-reference/", "W3C HTML Reference", "A fast, iframe-friendly HTML reference document.", "Technology"),
            ("https://player.vimeo.com/video/43408198", "Vimeo Embed", "A beautiful high definition video player embed.", "Art"),
            ("https://lite.cnn.com/", "CNN Lite", "A text-only, fast-loading, iframe-friendly version of CNN.", "News"),
            ("https://openlibrary.org/", "Open Library", "An open, editable library catalog, building towards a web page for every book ever published.", "Education")
        };

        // Clear existing data if requested
        var allWebsites = (await unitOfWork.Websites.GetAllAsync()).ToList();
        if (allWebsites.Any())
        {
            foreach (var w in allWebsites)
            {
                await unitOfWork.Websites.DeleteAsync(w);
            }
            await unitOfWork.CompleteAsync();
            await _redis.KeyDeleteAsync("websites_list");
            foreach (var cat in categoriesList)
            {
                await _redis.KeyDeleteAsync($"websites_list:category:{cat.Id}");
            }
        }

        var generatedWebsites = new List<Website>();
        foreach (var site in realWebsites)
        {
            var category = categoriesList.FirstOrDefault(c => c.Name == site.Category) ?? categoriesList[0];
            var website = new Website
            {
                Id = Guid.NewGuid(),
                Url = site.Url,
                Title = site.Title,
                Description = site.Description,
                SubmittedById = systemUser.Id,
                CreatedAt = DateTime.UtcNow,
                WebsiteCategories = new List<WebsiteCategory>
                {
                    new WebsiteCategory { CategoryId = category.Id }
                }
            };
            generatedWebsites.Add(website);
        }

        if (count > realWebsites.Count)
        {
            var searchBaseUrls = new[]
            {
                "https://en.wikipedia.org/wiki/Special:Search?search=",
                "https://archive.org/search.php?query=",
                "https://itch.io/search?q=",
                "https://dev.to/search?q=",
                "https://github.com/search?q=",
                "https://bandcamp.com/search?q="
            };

            var websiteFaker = new Faker<Website>()
                .RuleFor(w => w.Id, f => Guid.NewGuid())
                .RuleFor(w => w.Url, f => 
                {
                    var baseUrl = f.PickRandom(searchBaseUrls);
                    var term = Uri.EscapeDataString(f.Commerce.Department().ToLower());
                    return $"{baseUrl}{term}&ref={Guid.NewGuid():N}";
                })
                .RuleFor(w => w.Title, f => f.Company.CatchPhrase())
                .RuleFor(w => w.Description, f => f.Company.Bs())
                .RuleFor(w => w.SubmittedById, f => systemUser.Id)
                .RuleFor(w => w.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
                .RuleFor(w => w.WebsiteCategories, f => new List<WebsiteCategory> 
                { 
                    new WebsiteCategory 
                    { 
                        CategoryId = categoriesList[random.Next(categoriesList.Count)].Id 
                    } 
                });
            
            generatedWebsites.AddRange(websiteFaker.Generate(count - realWebsites.Count));
        }

        const int batchSize = 2500;
        for (int i = 0; i < generatedWebsites.Count; i += batchSize)
        {
            var batch = generatedWebsites.Skip(i).Take(batchSize).ToList();

            await unitOfWork.Websites.AddRangeAsync(batch);
            await unitOfWork.CompleteAsync();

            var redisValues = batch.Select(w => (RedisValue)JsonSerializer.Serialize(w.ToDto())).ToArray();
            await _redis.ListRightPushAsync("websites_list", redisValues);
            
            var categoryGroups = batch.SelectMany(w => w.WebsiteCategories.Select(wc => new { wc.CategoryId, Website = w }))
                                      .GroupBy(x => x.CategoryId);
                                      
            foreach (var group in categoryGroups)
            {
                var catRedisValues = group.Select(x => (RedisValue)JsonSerializer.Serialize(x.Website.ToDto())).ToArray();
                await _redis.ListRightPushAsync($"websites_list:category:{group.Key}", catRedisValues);
            }
        }

        return generatedWebsites.Count;
    }
}

