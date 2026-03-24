using TryMeTumble.Application.DTOs;
using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Application.Mappers
{
    public static class ReportMapperExtensions
    {
        public static ReportResponseDto ToDto(this Report report)
        {
            if (report == null) return null!;

            return new ReportResponseDto
            {
                Id = report.Id,
                WebsiteId = report.WebsiteId,
                WebsiteUrl = report.Website?.Url ?? string.Empty,
                Username = report.User?.Username ?? string.Empty,
                Reason = report.Reason,
                IsResolved = report.IsResolved,
                CreatedAt = report.CreatedAt
            };
        }
    }
}