using AssetFlow.OMS.Web.DTOs.AuditLogs;

namespace AssetFlow.OMS.Web.Services.Interfaces;

public interface IAuditLogService
{
    Task<List<AuditLogResponseDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
}
