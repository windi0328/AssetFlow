using AssetFlow.OMS.Web.Models;
using AssetFlow.OMS.Web.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssetFlow.OMS.Web.Data.Seed;

public static class AppDbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher<ApplicationUser> passwordHasher, CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);

        if (await context.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        ApplicationUser admin = new()
        {
            UserName = "admin",
            DisplayName = "System Admin",
            Role = UserRole.Admin,
            CreatedAtUtc = DateTime.UtcNow
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin@123");

        ApplicationUser user = new()
        {
            UserName = "user01",
            DisplayName = "General User",
            Role = UserRole.User,
            CreatedAtUtc = DateTime.UtcNow
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "User@123");

        await context.Users.AddRangeAsync([admin, user], cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        List<Equipment> equipments =
        [
            new Equipment
            {
                Name = "Dell Latitude 7440",
                Category = "Laptop",
                Status = EquipmentStatus.InUse,
                PurchaseDate = new DateTime(2024, 6, 12),
                Location = "Taipei IT Room",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new Equipment
            {
                Name = "Canon EOS R10",
                Category = "Camera",
                Status = EquipmentStatus.Available,
                PurchaseDate = new DateTime(2023, 10, 5),
                Location = "Media Cabinet",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new Equipment
            {
                Name = "Fluke 87V Multimeter",
                Category = "Tool",
                Status = EquipmentStatus.Maintenance,
                PurchaseDate = new DateTime(2022, 3, 18),
                Location = "Lab Shelf A",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            }
        ];

        await context.Equipments.AddRangeAsync(equipments, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        List<BorrowRecord> borrowRecords =
        [
            new BorrowRecord
            {
                EquipmentId = equipments[0].Id,
                UserId = user.Id,
                BorrowDateUtc = DateTime.UtcNow.AddDays(-4),
                DueDateUtc = DateTime.UtcNow.AddDays(3),
                IsReturned = false,
                Status = BorrowRecordStatus.Borrowed,
                Notes = "Weekly field visit"
            },
            new BorrowRecord
            {
                EquipmentId = equipments[1].Id,
                UserId = admin.Id,
                BorrowDateUtc = DateTime.UtcNow.AddDays(-12),
                DueDateUtc = DateTime.UtcNow.AddDays(-5),
                ReturnDateUtc = DateTime.UtcNow.AddDays(-4),
                IsReturned = true,
                Status = BorrowRecordStatus.Returned,
                Notes = "Marketing shooting"
            }
        ];

        await context.BorrowRecords.AddRangeAsync(borrowRecords, cancellationToken);

        await context.AuditLogs.AddRangeAsync(
        [
            new AuditLog
            {
                UserId = admin.Id,
                UserName = admin.UserName,
                Action = AuditAction.CreateEquipment,
                EntityName = nameof(Equipment),
                EntityId = equipments[0].Id.ToString(),
                Detail = "Seeded initial laptop asset.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-10)
            },
            new AuditLog
            {
                UserId = user.Id,
                UserName = user.UserName,
                Action = AuditAction.BorrowEquipment,
                EntityName = nameof(BorrowRecord),
                EntityId = equipments[0].Id.ToString(),
                Detail = "Seeded active borrow record.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-4)
            }
        ], cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
