using ReqFlow.Domain;

namespace ReqFlow.Application;

public sealed class RequestService(IRequestRepository repository, IUserRepository userRepository) : IRequestService
{
    public async Task<RequestDetailDto> CreateAsync(CreateRequestDto dto, AuthenticatedUser actor, CancellationToken cancellationToken)
    {
        var user = await GetActiveUserAsync(actor, cancellationToken);
        ValidateLength(dto.Title, nameof(dto.Title), 150);
        ValidateLength(dto.Description, nameof(dto.Description), 1000);

        Request request;
        try
        {
            request = new Request(dto.Title, dto.Description, user.Id, user.Email);
        }
        catch (ArgumentException exception)
        {
            throw new ValidationException(exception.Message);
        }

        await repository.AddAsync(request, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapDetail(request);
    }

    public async Task<IReadOnlyList<RequestListItemDto>> ListAsync(AuthenticatedUser actor, CancellationToken cancellationToken)
    {
        await GetActiveUserAsync(actor, cancellationToken);
        return (await repository.ListAsync(cancellationToken)).Select(MapListItem).ToList();
    }

    public async Task<RequestDetailDto> GetAsync(Guid id, AuthenticatedUser actor, CancellationToken cancellationToken)
    {
        await GetActiveUserAsync(actor, cancellationToken);
        return MapDetail(await GetEntityAsync(id, cancellationToken));
    }

    public async Task<RequestDetailDto> ApproveAsync(Guid id, AuthenticatedUser actor, CancellationToken cancellationToken)
    {
        var user = await GetReviewerAsync(actor, cancellationToken);
        var request = await GetEntityAsync(id, cancellationToken);

        try
        {
            request.Approve(user.Id, user.Email);
        }
        catch (ArgumentException exception)
        {
            throw new ValidationException(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            throw new ConflictException(exception.Message);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new ForbiddenException(exception.Message);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return MapDetail(request);
    }

    public async Task<RequestDetailDto> RejectAsync(Guid id, RejectRequestDto dto, AuthenticatedUser actor, CancellationToken cancellationToken)
    {
        var user = await GetReviewerAsync(actor, cancellationToken);
        ValidateLength(dto.Reason, nameof(dto.Reason), 500);
        var request = await GetEntityAsync(id, cancellationToken);

        try
        {
            request.Reject(user.Id, user.Email, dto.Reason);
        }
        catch (ArgumentException exception)
        {
            throw new ValidationException(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            throw new ConflictException(exception.Message);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new ForbiddenException(exception.Message);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return MapDetail(request);
    }

    private async Task<Request> GetEntityAsync(Guid id, CancellationToken cancellationToken) =>
        await repository.GetAsync(id, cancellationToken)
        ?? throw new NotFoundException($"Request '{id}' was not found.");

    private async Task<User> GetActiveUserAsync(AuthenticatedUser actor, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(actor.Id, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new ForbiddenException("The authenticated user is not active.");
        }

        return user;
    }

    private async Task<User> GetReviewerAsync(AuthenticatedUser actor, CancellationToken cancellationToken)
    {
        var user = await GetActiveUserAsync(actor, cancellationToken);
        if (user.Role is not (UserRole.Approver or UserRole.Admin))
        {
            throw new ForbiddenException("Only approvers or administrators can review requests.");
        }

        return user;
    }

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
            request.RequestedByUserId,
            request.Status.ToString(),
            request.CreatedAt,
            request.UpdatedAt,
            request.ApprovedRejectedBy,
            request.ApprovedRejectedByUserId,
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
