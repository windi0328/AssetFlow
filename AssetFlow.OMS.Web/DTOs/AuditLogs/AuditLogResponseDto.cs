namespace AssetFlow.OMS.Web.DTOs.AuditLogs;

public sealed class AuditLogResponseDto
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    public string Detail { get; set; } = string.Empty;

    public string CreatedAtUtc { get; set; } = string.Empty;
}
