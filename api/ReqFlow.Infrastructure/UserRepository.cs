using Microsoft.EntityFrameworkCore;
using ReqFlow.Application;
using ReqFlow.Domain;

namespace ReqFlow.Infrastructure;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<IReadOnlyList<User>> ListActiveAsync(CancellationToken cancellationToken) =>
        await dbContext.Users
            .AsNoTracking()
            .Where(user => user.IsActive)
            .OrderBy(user => user.DisplayName)
            .ToListAsync(cancellationToken);

    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
}
