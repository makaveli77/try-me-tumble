using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Domain.Interfaces
{
    public interface IReportRepository : IBaseRepository<Report>
    {
        Task<IEnumerable<Report>> GetUnresolvedReportsAsync(int page = 1, int pageSize = 20);
    }
}