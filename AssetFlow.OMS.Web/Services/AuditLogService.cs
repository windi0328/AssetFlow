using AssetFlow.OMS.Web.DTOs.AuditLogs;
using AssetFlow.OMS.Web.Repositories.Interfaces;
using AssetFlow.OMS.Web.Services.Interfaces;

namespace AssetFlow.OMS.Web.Services;

public sealed class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<List<AuditLogResponseDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        List<Models.AuditLog> logs = await _auditLogRepository.GetRecentAsync(take, cancellationToken);
        return logs.Select(x => x.ToDto()).ToList();
    }
}
