using AssetFlow.OMS.Web.DTOs.BorrowRecords;

namespace AssetFlow.OMS.Web.Services.Interfaces;

public interface IBorrowService
{
    Task<List<BorrowRecordResponseDto>> GetAllAsync(bool? returnedOnly, bool overdueOnly, int? userId, CancellationToken cancellationToken = default);

    Task<BorrowRecordResponseDto> BorrowAsync(BorrowCreateRequestDto request, int userId, string userName, CancellationToken cancellationToken = default);

    Task<BorrowRecordResponseDto> ReturnAsync(int borrowRecordId, ReturnBorrowRequestDto request, int userId, string userName, CancellationToken cancellationToken = default);

    Task<int> SyncOverdueRecordsAsync(CancellationToken cancellationToken = default);
}
