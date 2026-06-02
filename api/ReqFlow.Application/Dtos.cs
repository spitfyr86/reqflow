namespace ReqFlow.Application;

public sealed record CreateRequestDto(string Title, string Description);
public sealed record ApproveRequestDto();
public sealed record RejectRequestDto(string Reason);
public sealed record CurrentUserDto(Guid Id, string Email, string DisplayName, string Role);
public sealed record DemoLoginDto(Guid UserId);
public sealed record LoginResponseDto(string AccessToken, CurrentUserDto User);
public sealed record PendingRequestCountDto(int Count);

public sealed record RequestListItemDto(Guid Id, string Title, string RequestedBy, string Status, DateTime CreatedAt);

public sealed record RequestHistoryDto(
    Guid Id,
    string? FromStatus,
    string ToStatus,
    string ChangedBy,
    DateTime ChangedAt,
    string? Comment);

public sealed record RequestDetailDto(
    Guid Id,
    string Title,
    string Description,
    string RequestedBy,
    Guid RequestedByUserId,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? ApprovedRejectedBy,
    Guid? ApprovedRejectedByUserId,
    DateTime? ApprovedRejectedAt,
    string? RejectionReason,
    IReadOnlyList<RequestHistoryDto> History);
