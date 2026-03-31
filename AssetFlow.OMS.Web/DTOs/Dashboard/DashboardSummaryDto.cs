using AssetFlow.OMS.Web.DTOs.AuditLogs;
using AssetFlow.OMS.Web.DTOs.BorrowRecords;
using AssetFlow.OMS.Web.DTOs.Equipment;

namespace AssetFlow.OMS.Web.DTOs.Dashboard;

public sealed class DashboardSummaryDto
{
    public int TotalEquipmentCount { get; set; }

    public int AvailableEquipmentCount { get; set; }

    public int InUseEquipmentCount { get; set; }

    public int MaintenanceEquipmentCount { get; set; }

    public int ActiveBorrowCount { get; set; }

    public int OverdueBorrowCount { get; set; }

    public List<EquipmentResponseDto> EquipmentHighlights { get; set; } = new();

    public List<BorrowRecordResponseDto> RecentBorrowRecords { get; set; } = new();

    public List<AuditLogResponseDto> RecentLogs { get; set; } = new();
}
