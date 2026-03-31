using AssetFlow.OMS.Web.Data;
using AssetFlow.OMS.Web.Models;
using AssetFlow.OMS.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetFlow.OMS.Web.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<AuditLog>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        return _context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(AuditLog log, CancellationToken cancellationToken = default)
    {
        return _context.AuditLogs.AddAsync(log, cancellationToken).AsTask();
    }
}
