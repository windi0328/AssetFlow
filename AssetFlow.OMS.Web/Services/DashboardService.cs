using AssetFlow.OMS.Web.DTOs.Dashboard;
using AssetFlow.OMS.Web.Models.Enums;
using AssetFlow.OMS.Web.Services.Interfaces;

namespace AssetFlow.OMS.Web.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IEquipmentService _equipmentService;
    private readonly IBorrowService _borrowService;
    private readonly IAuditLogService _auditLogService;

    public DashboardService(
        IEquipmentService equipmentService,
        IBorrowService borrowService,
        IAuditLogService auditLogService)
    {
        _equipmentService = equipmentService;
        _borrowService = borrowService;
        _auditLogService = auditLogService;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        List<DTOs.Equipment.EquipmentResponseDto> equipments = await _equipmentService.GetAllAsync(null, null, null, cancellationToken);
        List<DTOs.BorrowRecords.BorrowRecordResponseDto> activeBorrows = await _borrowService.GetAllAsync(false, false, null, cancellationToken);
        List<DTOs.BorrowRecords.BorrowRecordResponseDto> overdueBorrows = await _borrowService.GetAllAsync(false, true, null, cancellationToken);
        List<DTOs.AuditLogs.AuditLogResponseDto> logs = await _auditLogService.GetRecentAsync(6, cancellationToken);

        return new DashboardSummaryDto
        {
            TotalEquipmentCount = equipments.Count,
            AvailableEquipmentCount = equipments.Count(x => x.Status == EquipmentStatus.Available.ToString()),
            InUseEquipmentCount = equipments.Count(x => x.Status == EquipmentStatus.InUse.ToString()),
            MaintenanceEquipmentCount = equipments.Count(x => x.Status == EquipmentStatus.Maintenance.ToString()),
            ActiveBorrowCount = activeBorrows.Count,
            OverdueBorrowCount = overdueBorrows.Count,
            EquipmentHighlights = equipments.Take(5).ToList(),
            RecentBorrowRecords = activeBorrows.Take(5).ToList(),
            RecentLogs = logs
        };
    }
}
