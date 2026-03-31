using AssetFlow.OMS.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetFlow.OMS.Web.Models;

public sealed class BorrowRecord
{
    public int Id { get; set; }

    public int EquipmentId { get; set; }

    public int UserId { get; set; }

    public DateTime BorrowDateUtc { get; set; }

    public DateTime DueDateUtc { get; set; }

    public DateTime? ReturnDateUtc { get; set; }

    public bool IsReturned { get; set; }

    public BorrowRecordStatus Status { get; set; } = BorrowRecordStatus.Borrowed;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime? OverdueReminderSentAtUtc { get; set; }

    public Equipment Equipment { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;
}
