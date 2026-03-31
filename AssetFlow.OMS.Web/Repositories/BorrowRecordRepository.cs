using AssetFlow.OMS.Web.Data;
using AssetFlow.OMS.Web.Models;
using AssetFlow.OMS.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetFlow.OMS.Web.Repositories;

public sealed class BorrowRecordRepository : IBorrowRecordRepository
{
    private readonly AppDbContext _context;

    public BorrowRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BorrowRecord>> GetAllAsync(bool? returnedOnly, bool overdueOnly, int? userId, CancellationToken cancellationToken = default)
    {
        IQueryable<BorrowRecord> query = _context.BorrowRecords
            .AsNoTracking()
            .Include(x => x.Equipment)
            .Include(x => x.User);

        if (returnedOnly.HasValue)
        {
            query = query.Where(x => x.IsReturned == returnedOnly.Value);
        }

        if (overdueOnly)
        {
            query = query.Where(x => !x.IsReturned && x.DueDateUtc < DateTime.UtcNow);
        }

        if (userId.HasValue)
        {
            query = query.Where(x => x.UserId == userId.Value);
        }

        return await query
            .OrderByDescending(x => x.BorrowDateUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<List<BorrowRecord>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        return _context.BorrowRecords
            .AsNoTracking()
            .Include(x => x.Equipment)
            .Include(x => x.User)
            .OrderByDescending(x => x.BorrowDateUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<BorrowRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.BorrowRecords
            .Include(x => x.Equipment)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<BorrowRecord?> GetActiveByEquipmentIdAsync(int equipmentId, CancellationToken cancellationToken = default)
    {
        return _context.BorrowRecords
            .Include(x => x.Equipment)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.EquipmentId == equipmentId && !x.IsReturned, cancellationToken);
    }

    public Task<bool> HasActiveRecordsForEquipmentAsync(int equipmentId, CancellationToken cancellationToken = default)
    {
        return _context.BorrowRecords.AnyAsync(x => x.EquipmentId == equipmentId && !x.IsReturned, cancellationToken);
    }

    public Task<List<BorrowRecord>> GetActiveOverdueAsync(DateTime utcNow, CancellationToken cancellationToken = default)
    {
        return _context.BorrowRecords
            .Include(x => x.Equipment)
            .Include(x => x.User)
            .Where(x => !x.IsReturned && x.DueDateUtc < utcNow)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(BorrowRecord record, CancellationToken cancellationToken = default)
    {
        return _context.BorrowRecords.AddAsync(record, cancellationToken).AsTask();
    }

    public void Update(BorrowRecord record)
    {
        _context.BorrowRecords.Update(record);
    }
}
