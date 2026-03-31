using AssetFlow.OMS.Web.Data;
using AssetFlow.OMS.Web.Models;
using AssetFlow.OMS.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetFlow.OMS.Web.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<ApplicationUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
    }

    public Task AddAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        return _context.Users.AddAsync(user, cancellationToken).AsTask();
    }
}
