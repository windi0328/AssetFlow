using AssetFlow.OMS.Web.DTOs.AuditLogs;
using AssetFlow.OMS.Web.DTOs.BorrowRecords;
using AssetFlow.OMS.Web.DTOs.Equipment;
using AssetFlow.OMS.Web.Models;

namespace AssetFlow.OMS.Web.Services;

public static class MappingExtensions
{
    public static EquipmentResponseDto ToDto(this Equipment equipment)
    {
        return new EquipmentResponseDto
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Category = equipment.Category,
            Status = equipment.Status.ToString(),
            PurchaseDate = equipment.PurchaseDate,
            Location = equipment.Location,
            UpdatedAtUtc = equipment.UpdatedAtUtc
        };
    }

    public static BorrowRecordResponseDto ToDto(this BorrowRecord record)
    {
        return new BorrowRecordResponseDto
        {
            Id = record.Id,
            EquipmentId = record.EquipmentId,
            EquipmentName = record.Equipment.Name,
            UserId = record.UserId,
            UserName = record.User.UserName,
            BorrowDateUtc = record.BorrowDateUtc.ToString("u"),
            DueDateUtc = record.DueDateUtc.ToString("u"),
            ReturnDateUtc = record.ReturnDateUtc?.ToString("u"),
            IsReturned = record.IsReturned,
            Status = record.Status.ToString(),
            Notes = record.Notes
        };
    }

    public static AuditLogResponseDto ToDto(this AuditLog log)
    {
        return new AuditLogResponseDto
        {
            Id = log.Id,
            UserName = log.UserName,
            Action = log.Action.ToString(),
            EntityName = log.EntityName,
            EntityId = log.EntityId,
            Detail = log.Detail,
            CreatedAtUtc = log.CreatedAtUtc.ToString("u")
        };
    }
}
