using AssetFlow.OMS.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetFlow.OMS.Web.DTOs.Equipment;

public sealed class EquipmentUpsertRequestDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public EquipmentStatus Status { get; set; }

    [Required]
    public DateTime PurchaseDate { get; set; }

    [Required]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;
}
