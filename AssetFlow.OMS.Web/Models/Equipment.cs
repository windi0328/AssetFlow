using AssetFlow.OMS.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetFlow.OMS.Web.Models;

public sealed class Equipment
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;

    public DateTime PurchaseDate { get; set; }

    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}
