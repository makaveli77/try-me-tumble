using TryMeTumble.Application.DTOs;

namespace TryMeTumble.Application.Interfaces
{
    public interface IWebsiteService
    {
        Task<WebsiteResponseDto?> GetRandomWebsiteAsync(Guid? categoryId = null);
        Task<WebsiteResponseDto> SubmitWebsiteAsync(WebsiteDto websiteDto, Guid userId);
        Task<bool> UpvoteWebsiteAsync(Guid websiteId, Guid userId);
        Task<bool> SaveWebsiteAsync(Guid websiteId, Guid userId);
        Task<bool> ReportWebsiteAsync(Guid websiteId, Guid userId, string reason);
        Task<IEnumerable<ReportResponseDto>> GetUnresolvedReportsAsync(int page = 1, int pageSize = 20);
        Task<WebsiteResponseDto?> GetWebsiteByIdAsync(Guid websiteId);
        Task<IEnumerable<WebsiteResponseDto>> GetWebsitesByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 20);
    }
}