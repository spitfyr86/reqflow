using ReqFlow.Domain;

namespace ReqFlow.Application;

public sealed class RequestService(IRequestRepository repository) : IRequestService
{
    public async Task<RequestDetailDto> CreateAsync(CreateRequestDto dto, CancellationToken cancellationToken)
    {
        ValidateLength(dto.Title, nameof(dto.Title), 150);
        ValidateLength(dto.Description, nameof(dto.Description), 1000);
        ValidateLength(dto.RequestedBy, nameof(dto.RequestedBy), 100);

        Request request;
        try
        {
            request = new Request(dto.Title, dto.Description, dto.RequestedBy);
        }
        catch (ArgumentException exception)
        {
            throw new ValidationException(exception.Message);
        }

        await repository.AddAsync(request, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapDetail(request);
    }

    public async Task<IReadOnlyList<RequestListItemDto>> ListAsync(CancellationToken cancellationToken) =>
        (await repository.ListAsync(cancellationToken)).Select(MapListItem).ToList();

    public async Task<RequestDetailDto> GetAsync(Guid id, CancellationToken cancellationToken) =>
        MapDetail(await GetEntityAsync(id, cancellationToken));

    public async Task<RequestDetailDto> ApproveAsync(Guid id, ApproveRequestDto dto, CancellationToken cancellationToken)
    {
        ValidateLength(dto.ChangedBy, nameof(dto.ChangedBy), 100);
        var request = await GetEntityAsync(id, cancellationToken);

        try
        {
            request.Approve(dto.ChangedBy);
        }
        catch (ArgumentException exception)
        {
            throw new ValidationException(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            throw new ConflictException(exception.Message);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return MapDetail(request);
    }

    public async Task<RequestDetailDto> RejectAsync(Guid id, RejectRequestDto dto, CancellationToken cancellationToken)
    {
        ValidateLength(dto.ChangedBy, nameof(dto.ChangedBy), 100);
        ValidateLength(dto.Reason, nameof(dto.Reason), 500);
        var request = await GetEntityAsync(id, cancellationToken);

        try
        {
            request.Reject(dto.ChangedBy, dto.Reason);
        }
        catch (ArgumentException exception)
        {
            throw new ValidationException(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            throw new ConflictException(exception.Message);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return MapDetail(request);
    }

    private async Task<Request> GetEntityAsync(Guid id, CancellationToken cancellationToken) =>
        await repository.GetAsync(id, cancellationToken)
        ?? throw new NotFoundException($"Request '{id}' was not found.");

    private static void ValidateLength(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException($"{field} is required.");
        }

        if (value.Trim().Length > maxLength)
        {
            throw new ValidationException($"{field} must be {maxLength} characters or fewer.");
        }
    }

    private static RequestListItemDto MapListItem(Request request) =>
        new(request.Id, request.Title, request.RequestedBy, request.Status.ToString(), request.CreatedAt);

    private static RequestDetailDto MapDetail(Request request) =>
        new(
            request.Id,
            request.Title,
            request.Description,
            request.RequestedBy,
            request.Status.ToString(),
            request.CreatedAt,
            request.UpdatedAt,
            request.ApprovedRejectedBy,
            request.ApprovedRejectedAt,
            request.RejectionReason,
            request.StatusHistory
                .OrderBy(history => history.ChangedAt)
                .Select(history => new RequestHistoryDto(
                    history.Id,
                    history.FromStatus?.ToString(),
                    history.ToStatus.ToString(),
                    history.ChangedBy,
                    history.ChangedAt,
                    history.Comment))
                .ToList());
}
