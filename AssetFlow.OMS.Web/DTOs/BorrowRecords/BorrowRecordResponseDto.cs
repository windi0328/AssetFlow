namespace AssetFlow.OMS.Web.DTOs.BorrowRecords;

public sealed class BorrowRecordResponseDto
{
    public int Id { get; set; }

    public int EquipmentId { get; set; }

    public string EquipmentName { get; set; } = string.Empty;

    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string BorrowDateUtc { get; set; } = string.Empty;

    public string DueDateUtc { get; set; } = string.Empty;

    public string? ReturnDateUtc { get; set; }

    public bool IsReturned { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
