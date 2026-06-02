namespace ReqFlow.Application;

public sealed record CreateRequestDto(string Title, string Description, string RequestedBy);
public sealed record ApproveRequestDto(string ChangedBy);
public sealed record RejectRequestDto(string ChangedBy, string Reason);

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
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? ApprovedRejectedBy,
    DateTime? ApprovedRejectedAt,
    string? RejectionReason,
    IReadOnlyList<RequestHistoryDto> History);
