using ReqFlow.Domain;

namespace ReqFlow.Application;

public interface IRequestRepository
{
    Task AddAsync(Request request, CancellationToken cancellationToken);
    Task<IReadOnlyList<Request>> ListAsync(CancellationToken cancellationToken);
    Task<Request?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IUserRepository
{
    Task<IReadOnlyList<User>> ListActiveAsync(CancellationToken cancellationToken);
    Task<User?> GetAsync(Guid id, CancellationToken cancellationToken);
}

public sealed record AuthenticatedUser(Guid Id);

public interface IRequestService
{
    Task<RequestDetailDto> CreateAsync(CreateRequestDto dto, AuthenticatedUser actor, CancellationToken cancellationToken);
    Task<IReadOnlyList<RequestListItemDto>> ListAsync(AuthenticatedUser actor, CancellationToken cancellationToken);
    Task<RequestDetailDto> GetAsync(Guid id, AuthenticatedUser actor, CancellationToken cancellationToken);
    Task<RequestDetailDto> ApproveAsync(Guid id, AuthenticatedUser actor, CancellationToken cancellationToken);
    Task<RequestDetailDto> RejectAsync(Guid id, RejectRequestDto dto, AuthenticatedUser actor, CancellationToken cancellationToken);
}
