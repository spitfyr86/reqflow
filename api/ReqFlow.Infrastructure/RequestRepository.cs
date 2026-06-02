using Microsoft.EntityFrameworkCore;
using ReqFlow.Application;
using ReqFlow.Domain;

namespace ReqFlow.Infrastructure;

public sealed class RequestRepository(AppDbContext dbContext) : IRequestRepository
{
    public async Task AddAsync(Request request, CancellationToken cancellationToken) =>
        await dbContext.Requests.AddAsync(request, cancellationToken);

    public async Task<IReadOnlyList<Request>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Requests.AsNoTracking().OrderByDescending(request => request.CreatedAt).ToListAsync(cancellationToken);

    public async Task<Request?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Requests.Include(request => request.StatusHistory).SingleOrDefaultAsync(request => request.Id == id, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConflictException("The request was changed by another user. Refresh and try again.", exception);
        }
    }
}
