using System.ComponentModel.DataAnnotations;

namespace AssetFlow.OMS.Web.DTOs.BorrowRecords;

public sealed class BorrowCreateRequestDto
{
    [Range(1, int.MaxValue)]
    public int EquipmentId { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
