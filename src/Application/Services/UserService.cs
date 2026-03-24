using TryMeTumble.Application.DTOs;
using TryMeTumble.Application.Interfaces;
using TryMeTumble.Application.Mappers;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return users.Select(u => u.ToProfileDto());
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return user?.ToProfileDto();
        }

        public async Task<IEnumerable<SavedWebsiteDto>> GetUserSavedWebsitesAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            var saves = await _unitOfWork.SavedWebsites.GetByUserIdAsync(userId, page, pageSize);
            return saves.Select(s => s.ToDto());
        }

        public async Task<IEnumerable<UpvotedWebsiteDto>> GetUserUpvotedWebsitesAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            var upvotes = await _unitOfWork.Upvotes.GetByUserIdAsync(userId, page, pageSize);
            return upvotes.Select(u => u.ToDto());
        }
    }
}