namespace AssetFlow.OMS.Web.DTOs.Equipment;

public sealed class EquipmentResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime PurchaseDate { get; set; }

    public string Location { get; set; } = string.Empty;

    public DateTime UpdatedAtUtc { get; set; }
}
