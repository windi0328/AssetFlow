using AssetFlow.OMS.Web.DTOs.Dashboard;

namespace AssetFlow.OMS.Web.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
