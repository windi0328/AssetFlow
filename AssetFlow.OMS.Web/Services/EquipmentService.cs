using AssetFlow.OMS.Web.DTOs.Equipment;
using AssetFlow.OMS.Web.Exceptions;
using AssetFlow.OMS.Web.Models;
using AssetFlow.OMS.Web.Models.Enums;
using AssetFlow.OMS.Web.Repositories.Interfaces;
using AssetFlow.OMS.Web.Services.Interfaces;

namespace AssetFlow.OMS.Web.Services;

public sealed class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IBorrowRecordRepository _borrowRecordRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentService(
        IEquipmentRepository equipmentRepository,
        IBorrowRecordRepository borrowRecordRepository,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork)
    {
        _equipmentRepository = equipmentRepository;
        _borrowRecordRepository = borrowRecordRepository;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<EquipmentResponseDto>> GetAllAsync(string? category, EquipmentStatus? status, string? keyword, CancellationToken cancellationToken = default)
    {
        List<Equipment> equipments = await _equipmentRepository.GetAllAsync(category, status, keyword, cancellationToken);
        return equipments.Select(x => x.ToDto()).ToList();
    }

    public async Task<EquipmentResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        Equipment equipment = await _equipmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Equipment {id} was not found.");

        return equipment.ToDto();
    }

    public async Task<EquipmentResponseDto> CreateAsync(EquipmentUpsertRequestDto request, int userId, string userName, CancellationToken cancellationToken = default)
    {
        if (request.PurchaseDate.Date > DateTime.UtcNow.Date)
        {
            throw new BadRequestException("Purchase date cannot be in the future.");
        }

        if (await _equipmentRepository.ExistsByNameAsync(request.Name.Trim(), cancellationToken))
        {
            throw new ConflictException("Equipment name already exists.");
        }

        Equipment equipment = new()
        {
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            Status = request.Status,
            PurchaseDate = request.PurchaseDate.Date,
            Location = request.Location.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _equipmentRepository.AddAsync(equipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditLogRepository.AddAsync(CreateLog(userId, userName, AuditAction.CreateEquipment, equipment.Id.ToString(), $"Created equipment {equipment.Name}."), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return equipment.ToDto();
    }

    public async Task<EquipmentResponseDto> UpdateAsync(int id, EquipmentUpsertRequestDto request, int userId, string userName, CancellationToken cancellationToken = default)
    {
        Equipment equipment = await _equipmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Equipment {id} was not found.");

        if (request.PurchaseDate.Date > DateTime.UtcNow.Date)
        {
            throw new BadRequestException("Purchase date cannot be in the future.");
        }

        if (equipment.Status == EquipmentStatus.InUse && request.Status != EquipmentStatus.InUse)
        {
            bool hasActiveBorrow = await _borrowRecordRepository.HasActiveRecordsForEquipmentAsync(id, cancellationToken);
            if (hasActiveBorrow)
            {
                throw new ConflictException("Equipment status cannot be changed while it is actively borrowed.");
            }
        }

        equipment.Name = request.Name.Trim();
        equipment.Category = request.Category.Trim();
        equipment.Status = request.Status;
        equipment.PurchaseDate = request.PurchaseDate.Date;
        equipment.Location = request.Location.Trim();
        equipment.UpdatedAtUtc = DateTime.UtcNow;

        _equipmentRepository.Update(equipment);
        await _auditLogRepository.AddAsync(CreateLog(userId, userName, AuditAction.UpdateEquipment, equipment.Id.ToString(), $"Updated equipment {equipment.Name}."), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return equipment.ToDto();
    }

    public async Task DeleteAsync(int id, int userId, string userName, CancellationToken cancellationToken = default)
    {
        Equipment equipment = await _equipmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Equipment {id} was not found.");

        if (await _borrowRecordRepository.HasActiveRecordsForEquipmentAsync(id, cancellationToken))
        {
            throw new ConflictException("Equipment with active borrow records cannot be deleted.");
        }

        _equipmentRepository.Remove(equipment);
        await _auditLogRepository.AddAsync(CreateLog(userId, userName, AuditAction.DeleteEquipment, id.ToString(), $"Deleted equipment {equipment.Name}."), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static AuditLog CreateLog(int userId, string userName, AuditAction action, string entityId, string detail)
    {
        return new AuditLog
        {
            UserId = userId,
            UserName = userName,
            Action = action,
            EntityName = nameof(Equipment),
            EntityId = entityId,
            Detail = detail,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
