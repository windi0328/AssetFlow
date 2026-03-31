using AssetFlow.OMS.Web.Data;
using AssetFlow.OMS.Web.Models;
using AssetFlow.OMS.Web.Models.Enums;
using AssetFlow.OMS.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetFlow.OMS.Web.Repositories;

public sealed class EquipmentRepository : IEquipmentRepository
{
    private readonly AppDbContext _context;

    public EquipmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Equipment>> GetAllAsync(string? category, EquipmentStatus? status, string? keyword, CancellationToken cancellationToken = default)
    {
        IQueryable<Equipment> query = _context.Equipments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(x => x.Category == category);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword) || x.Location.Contains(keyword));
        }

        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Equipments
            .Include(x => x.BorrowRecords.OrderByDescending(b => b.BorrowDateUtc).Take(5))
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _context.Equipments.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public Task AddAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        return _context.Equipments.AddAsync(equipment, cancellationToken).AsTask();
    }

    public void Update(Equipment equipment)
    {
        _context.Equipments.Update(equipment);
    }

    public void Remove(Equipment equipment)
    {
        _context.Equipments.Remove(equipment);
    }
}
