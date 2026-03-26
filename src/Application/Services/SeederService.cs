using Bogus;
using StackExchange.Redis;
using System.Text.Json;
using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;
using TryMeTumble.Application.Mappers;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Application.Services
{
    public class SeederService : ISeederService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDatabase _redis;

        public SeederService(IUnitOfWork unitOfWork, IConnectionMultiplexer redis)
        {
            _unitOfWork = unitOfWork;
            _redis = redis.GetDatabase();
        }

        public async Task<int> SeedWebsitesAsync(int count)
        {
            var systemUser = await _unitOfWork.Users.GetByUsernameAsync("system_seeder");
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
                await _unitOfWork.Users.AddAsync(systemUser);
                await _unitOfWork.CompleteAsync();
            }

            var categoryNames = new[] { "Technology", "Science", "Art", "Gaming", "Music", "Education", "News", "Entertainment" };
            var existingCategories = await _unitOfWork.Categories.GetAllAsync();
            var categoriesList = existingCategories.ToList();

            if (!categoriesList.Any())
            {
                foreach (var name in categoryNames)
                {
                    var cat = new Category { Id = Guid.NewGuid(), Name = name, Description = $"{name} websites" };
                    await _unitOfWork.Categories.AddAsync(cat);
                    categoriesList.Add(cat);
                }
                await _unitOfWork.CompleteAsync();
            }

            var random = new Random();

            var websiteFaker = new Faker<Website>()
                .RuleFor(w => w.Id, f => Guid.NewGuid())
                .RuleFor(w => w.Url, f => f.Internet.Url() + "/" + f.Random.AlphaNumeric(8))
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

            var generatedWebsites = websiteFaker.Generate(count);

            const int batchSize = 2500;
            for (int i = 0; i < generatedWebsites.Count; i += batchSize)
            {
                var batch = generatedWebsites.Skip(i).Take(batchSize).ToList();

                await _unitOfWork.Websites.AddRangeAsync(batch);
                await _unitOfWork.CompleteAsync();

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
}
