using AssetFlow.OMS.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetFlow.OMS.Web.Models;

public sealed class AuditLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    public AuditAction Action { get; set; }

    [MaxLength(50)]
    public string EntityName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string EntityId { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Detail { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
