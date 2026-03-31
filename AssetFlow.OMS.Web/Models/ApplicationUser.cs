using AssetFlow.OMS.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetFlow.OMS.Web.Models;

public sealed class ApplicationUser
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}
