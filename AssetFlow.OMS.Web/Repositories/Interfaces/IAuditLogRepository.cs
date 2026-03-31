using AssetFlow.OMS.Web.Models;

namespace AssetFlow.OMS.Web.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task<List<AuditLog>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    Task AddAsync(AuditLog log, CancellationToken cancellationToken = default);
}
