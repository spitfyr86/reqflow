namespace ReqFlow.Domain;

public sealed class RequestStatusHistory
{
    private RequestStatusHistory() { }

    internal RequestStatusHistory(Guid requestId, RequestStatus? fromStatus, RequestStatus toStatus, string changedBy, string? comment, DateTime changedAt)
    {
        Id = Guid.NewGuid();
        RequestId = requestId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        ChangedBy = changedBy;
        Comment = comment;
        ChangedAt = changedAt;
    }

    public Guid Id { get; private set; }
    public Guid RequestId { get; private set; }
    public RequestStatus? FromStatus { get; private set; }
    public RequestStatus ToStatus { get; private set; }
    public string ChangedBy { get; private set; } = string.Empty;
    public DateTime ChangedAt { get; private set; }
    public string? Comment { get; private set; }
}
