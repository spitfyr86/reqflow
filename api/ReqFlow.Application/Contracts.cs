using ReqFlow.Domain;

namespace ReqFlow.Application;

public interface IRequestRepository
{
    Task AddAsync(Request request, CancellationToken cancellationToken);
    Task<IReadOnlyList<Request>> ListAsync(CancellationToken cancellationToken);
    Task<Request?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IRequestService
{
    Task<RequestDetailDto> CreateAsync(CreateRequestDto dto, CancellationToken cancellationToken);
    Task<IReadOnlyList<RequestListItemDto>> ListAsync(CancellationToken cancellationToken);
    Task<RequestDetailDto> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<RequestDetailDto> ApproveAsync(Guid id, ApproveRequestDto dto, CancellationToken cancellationToken);
    Task<RequestDetailDto> RejectAsync(Guid id, RejectRequestDto dto, CancellationToken cancellationToken);
}
