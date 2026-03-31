using AssetFlow.OMS.Web.DTOs.Equipment;
using AssetFlow.OMS.Web.Models.Enums;

namespace AssetFlow.OMS.Web.Services.Interfaces;

public interface IEquipmentService
{
    Task<List<EquipmentResponseDto>> GetAllAsync(string? category, EquipmentStatus? status, string? keyword, CancellationToken cancellationToken = default);

    Task<EquipmentResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<EquipmentResponseDto> CreateAsync(EquipmentUpsertRequestDto request, int userId, string userName, CancellationToken cancellationToken = default);

    Task<EquipmentResponseDto> UpdateAsync(int id, EquipmentUpsertRequestDto request, int userId, string userName, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, int userId, string userName, CancellationToken cancellationToken = default);
}
