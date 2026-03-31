using AssetFlow.OMS.Web.Models;

namespace AssetFlow.OMS.Web.Repositories.Interfaces;

public interface IBorrowRecordRepository
{
    Task<List<BorrowRecord>> GetAllAsync(bool? returnedOnly, bool overdueOnly, int? userId, CancellationToken cancellationToken = default);

    Task<List<BorrowRecord>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    Task<BorrowRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BorrowRecord?> GetActiveByEquipmentIdAsync(int equipmentId, CancellationToken cancellationToken = default);

    Task<bool> HasActiveRecordsForEquipmentAsync(int equipmentId, CancellationToken cancellationToken = default);

    Task<List<BorrowRecord>> GetActiveOverdueAsync(DateTime utcNow, CancellationToken cancellationToken = default);

    Task AddAsync(BorrowRecord record, CancellationToken cancellationToken = default);

    void Update(BorrowRecord record);
}
