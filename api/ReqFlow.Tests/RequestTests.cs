using ReqFlow.Domain;

namespace ReqFlow.Tests;

public sealed class RequestTests
{
    [Fact]
    public void CreateRequest_StartsPendingAndRecordsHistory()
    {
        var request = NewRequest();

        Assert.Equal(RequestStatus.Pending, request.Status);
        var history = Assert.Single(request.StatusHistory);
        Assert.Null(history.FromStatus);
        Assert.Equal(RequestStatus.Pending, history.ToStatus);
    }

    [Fact]
    public void ApprovePendingRequest_Succeeds()
    {
        var request = NewRequest();

        request.Approve("team.lead@example.com");

        Assert.Equal(RequestStatus.Approved, request.Status);
        Assert.Equal("team.lead@example.com", request.ApprovedRejectedBy);
        Assert.Equal(2, request.StatusHistory.Count);
    }

    [Fact]
    public void RejectPendingRequestWithReason_Succeeds()
    {
        var request = NewRequest();

        request.Reject("team.lead@example.com", "Budget is not available.");

        Assert.Equal(RequestStatus.Rejected, request.Status);
        Assert.Equal("Budget is not available.", request.RejectionReason);
        Assert.Equal(2, request.StatusHistory.Count);
    }

    [Fact]
    public void RejectPendingRequestWithoutReason_Fails()
    {
        var request = NewRequest();

        Assert.Throws<ArgumentException>(() => request.Reject("team.lead@example.com", " "));
        Assert.Equal(RequestStatus.Pending, request.Status);
        Assert.Single(request.StatusHistory);
    }

    [Fact]
    public void ApproveRejectedRequest_Fails()
    {
        var request = NewRequest();
        request.Reject("team.lead@example.com", "Not needed.");

        Assert.Throws<InvalidOperationException>(() => request.Approve("team.lead@example.com"));
    }

    [Fact]
    public void RejectApprovedRequest_Fails()
    {
        var request = NewRequest();
        request.Approve("team.lead@example.com");

        Assert.Throws<InvalidOperationException>(() => request.Reject("team.lead@example.com", "Changed my mind."));
    }

    private static Request NewRequest() =>
        new("New laptop", "Replacement laptop for a developer.", "requester@example.com");
}
