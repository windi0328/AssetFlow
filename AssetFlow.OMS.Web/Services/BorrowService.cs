using AssetFlow.OMS.Web.DTOs.BorrowRecords;
using AssetFlow.OMS.Web.Exceptions;
using AssetFlow.OMS.Web.Models;
using AssetFlow.OMS.Web.Models.Enums;
using AssetFlow.OMS.Web.Repositories.Interfaces;
using AssetFlow.OMS.Web.Services.Interfaces;

namespace AssetFlow.OMS.Web.Services;

public sealed class BorrowService : IBorrowService
{
    private readonly IBorrowRecordRepository _borrowRecordRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BorrowService(
        IBorrowRecordRepository borrowRecordRepository,
        IEquipmentRepository equipmentRepository,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork)
    {
        _borrowRecordRepository = borrowRecordRepository;
        _equipmentRepository = equipmentRepository;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BorrowRecordResponseDto>> GetAllAsync(bool? returnedOnly, bool overdueOnly, int? userId, CancellationToken cancellationToken = default)
    {
        await SyncOverdueRecordsAsync(cancellationToken);
        List<BorrowRecord> records = await _borrowRecordRepository.GetAllAsync(returnedOnly, overdueOnly, userId, cancellationToken);
        return records.Select(x => x.ToDto()).ToList();
    }

    public async Task<BorrowRecordResponseDto> BorrowAsync(BorrowCreateRequestDto request, int userId, string userName, CancellationToken cancellationToken = default)
    {
        if (request.DueDate.Date <= DateTime.UtcNow.Date)
        {
            throw new BadRequestException("Due date must be later than today.");
        }

        Equipment equipment = await _equipmentRepository.GetByIdAsync(request.EquipmentId, cancellationToken)
            ?? throw new NotFoundException($"Equipment {request.EquipmentId} was not found.");

        if (equipment.Status == EquipmentStatus.Maintenance)
        {
            throw new ConflictException("Equipment in maintenance cannot be borrowed.");
        }

        BorrowRecord? activeRecord = await _borrowRecordRepository.GetActiveByEquipmentIdAsync(request.EquipmentId, cancellationToken);
        if (activeRecord is not null)
        {
            throw new ConflictException("Equipment is already borrowed.");
        }

        BorrowRecord record = new()
        {
            EquipmentId = equipment.Id,
            UserId = userId,
            BorrowDateUtc = DateTime.UtcNow,
            DueDateUtc = request.DueDate.ToUniversalTime(),
            IsReturned = false,
            Status = BorrowRecordStatus.Borrowed,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };

        equipment.Status = EquipmentStatus.InUse;
        equipment.UpdatedAtUtc = DateTime.UtcNow;

        await _borrowRecordRepository.AddAsync(record, cancellationToken);
        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            UserName = userName,
            Action = AuditAction.BorrowEquipment,
            EntityName = nameof(BorrowRecord),
            EntityId = equipment.Id.ToString(),
            Detail = $"Borrowed equipment {equipment.Name} until {record.DueDateUtc:u}.",
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        BorrowRecord savedRecord = await _borrowRecordRepository.GetByIdAsync(record.Id, cancellationToken)
            ?? throw new NotFoundException("Borrow record was created but could not be reloaded.");

        return savedRecord.ToDto();
    }

    public async Task<BorrowRecordResponseDto> ReturnAsync(int borrowRecordId, ReturnBorrowRequestDto request, int userId, string userName, CancellationToken cancellationToken = default)
    {
        BorrowRecord record = await _borrowRecordRepository.GetByIdAsync(borrowRecordId, cancellationToken)
            ?? throw new NotFoundException($"Borrow record {borrowRecordId} was not found.");

        if (record.IsReturned)
        {
            throw new ConflictException("This borrow record has already been returned.");
        }

        record.IsReturned = true;
        record.ReturnDateUtc = DateTime.UtcNow;
        record.Status = BorrowRecordStatus.Returned;
        record.Notes = string.IsNullOrWhiteSpace(request.Notes)
            ? record.Notes
            : $"{record.Notes}{Environment.NewLine}{request.Notes}".Trim();

        record.Equipment.Status = request.SendToMaintenance ? EquipmentStatus.Maintenance : EquipmentStatus.Available;
        record.Equipment.UpdatedAtUtc = DateTime.UtcNow;

        _borrowRecordRepository.Update(record);
        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            UserName = userName,
            Action = AuditAction.ReturnEquipment,
            EntityName = nameof(BorrowRecord),
            EntityId = record.Id.ToString(),
            Detail = request.SendToMaintenance
                ? $"Returned equipment {record.Equipment.Name} and sent it to maintenance."
                : $"Returned equipment {record.Equipment.Name}.",
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return record.ToDto();
    }

    public async Task<int> SyncOverdueRecordsAsync(CancellationToken cancellationToken = default)
    {
        List<BorrowRecord> overdueRecords = await _borrowRecordRepository.GetActiveOverdueAsync(DateTime.UtcNow, cancellationToken);
        int updatedCount = 0;

        foreach (BorrowRecord record in overdueRecords)
        {
            if (record.Status != BorrowRecordStatus.Overdue)
            {
                record.Status = BorrowRecordStatus.Overdue;
                updatedCount++;
            }

            if (record.OverdueReminderSentAtUtc is null)
            {
                record.OverdueReminderSentAtUtc = DateTime.UtcNow;
                await _auditLogRepository.AddAsync(new AuditLog
                {
                    UserId = record.UserId,
                    UserName = record.User.UserName,
                    Action = AuditAction.MarkBorrowOverdue,
                    EntityName = nameof(BorrowRecord),
                    EntityId = record.Id.ToString(),
                    Detail = $"Borrow record {record.Id} for {record.Equipment.Name} is overdue.",
                    CreatedAtUtc = DateTime.UtcNow
                }, cancellationToken);
            }

            _borrowRecordRepository.Update(record);
        }

        if (overdueRecords.Count > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return updatedCount;
    }
}
