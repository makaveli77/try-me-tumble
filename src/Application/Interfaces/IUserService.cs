using TryMeTumble.Application.DTOs;

namespace TryMeTumble.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserProfileDto>> GetAllUsersAsync();
        Task<UserProfileDto?> GetUserProfileAsync(Guid id);
        Task<IEnumerable<SavedWebsiteDto>> GetUserSavedWebsitesAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<IEnumerable<UpvotedWebsiteDto>> GetUserUpvotedWebsitesAsync(Guid userId, int page = 1, int pageSize = 20);
    }
}