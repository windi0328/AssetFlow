using AssetFlow.OMS.Web.Models;

namespace AssetFlow.OMS.Web.Repositories.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    Task AddAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}
