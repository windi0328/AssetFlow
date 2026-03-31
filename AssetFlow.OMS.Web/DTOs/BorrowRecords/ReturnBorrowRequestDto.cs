using System.ComponentModel.DataAnnotations;

namespace AssetFlow.OMS.Web.DTOs.BorrowRecords;

public sealed class ReturnBorrowRequestDto
{
    public bool SendToMaintenance { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
