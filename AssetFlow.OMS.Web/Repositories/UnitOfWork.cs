using AssetFlow.OMS.Web.Data;
using AssetFlow.OMS.Web.Repositories.Interfaces;

namespace AssetFlow.OMS.Web.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
