using Microsoft.EntityFrameworkCore;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Domain.Interfaces;

namespace TryMeTumble.Infrastructure.Persistence.Repositories;

public class ReportRepository(DataContext context) : BaseRepository<Report>(context), IReportRepository
{
    public async Task<IEnumerable<Report>> GetUnresolvedReportsAsync(int page = 1, int pageSize = 20) =>
        await _context.Reports
            .Include(r => r.Website)
            .Include(r => r.User)
            .Where(r => !r.IsResolved)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
}
