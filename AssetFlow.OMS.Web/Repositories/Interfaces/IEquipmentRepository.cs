using AssetFlow.OMS.Web.Models;
using AssetFlow.OMS.Web.Models.Enums;

namespace AssetFlow.OMS.Web.Repositories.Interfaces;

public interface IEquipmentRepository
{
    Task<List<Equipment>> GetAllAsync(string? category, EquipmentStatus? status, string? keyword, CancellationToken cancellationToken = default);

    Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    Task AddAsync(Equipment equipment, CancellationToken cancellationToken = default);

    void Update(Equipment equipment);

    void Remove(Equipment equipment);
}
