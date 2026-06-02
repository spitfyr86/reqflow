namespace ReqFlow.Domain;

public sealed class Request
{
    private readonly List<RequestStatusHistory> _statusHistory = [];

    private Request() { }

    public Request(string title, string description, string requestedBy, DateTime? createdAt = null)
    {
        Id = Guid.NewGuid();
        Title = Require(title, nameof(title));
        Description = Require(description, nameof(description));
        RequestedBy = Require(requestedBy, nameof(requestedBy));
        Status = RequestStatus.Pending;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        _statusHistory.Add(new RequestStatusHistory(Id, null, Status, RequestedBy, "Request created", CreatedAt));
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public RequestStatus Status { get; private set; }
    public string RequestedBy { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? ApprovedRejectedBy { get; private set; }
    public DateTime? ApprovedRejectedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<RequestStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    public void Approve(string changedBy, DateTime? changedAt = null)
    {
        EnsurePending();
        var timestamp = changedAt ?? DateTime.UtcNow;
        var actor = Require(changedBy, nameof(changedBy));

        Status = RequestStatus.Approved;
        UpdatedAt = timestamp;
        ApprovedRejectedBy = actor;
        ApprovedRejectedAt = timestamp;
        _statusHistory.Add(new RequestStatusHistory(Id, RequestStatus.Pending, Status, actor, "Request approved", timestamp));
    }

    public void Reject(string changedBy, string reason, DateTime? changedAt = null)
    {
        EnsurePending();
        var timestamp = changedAt ?? DateTime.UtcNow;
        var actor = Require(changedBy, nameof(changedBy));
        var rejectionReason = Require(reason, nameof(reason));

        Status = RequestStatus.Rejected;
        UpdatedAt = timestamp;
        ApprovedRejectedBy = actor;
        ApprovedRejectedAt = timestamp;
        RejectionReason = rejectionReason;
        _statusHistory.Add(new RequestStatusHistory(Id, RequestStatus.Pending, Status, actor, rejectionReason, timestamp));
    }

    private void EnsurePending()
    {
        if (Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException($"A request with status '{Status}' cannot transition again.");
        }
    }

    private static string Require(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{name} is required.", name);
        }

        return value.Trim();
    }
}
