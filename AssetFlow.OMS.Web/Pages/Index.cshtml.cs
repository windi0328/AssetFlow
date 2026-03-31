using AssetFlow.OMS.Web.DTOs.Dashboard;
using AssetFlow.OMS.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetFlow.OMS.Web.Pages;

public sealed class IndexModel : PageModel
{
    private readonly IDashboardService _dashboardService;

    public IndexModel(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public DashboardSummaryDto Summary { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Summary = await _dashboardService.GetSummaryAsync(cancellationToken);
    }
}
