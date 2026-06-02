USE ReqFlow;
GO

DECLARE @PendingRequestId UNIQUEIDENTIFIER = NEWID();
DECLARE @ApprovedRequestId UNIQUEIDENTIFIER = NEWID();
DECLARE @CreatedAt DATETIME2 = SYSUTCDATETIME();

INSERT INTO dbo.Requests (Id, Title, Description, Status, RequestedBy, CreatedAt)
VALUES
    (@PendingRequestId, N'New laptop', N'Replacement laptop for a developer.', N'Pending', N'alex@example.com', @CreatedAt),
    (@ApprovedRequestId, N'Training budget', N'Cloud architecture course registration.', N'Approved', N'sam@example.com', @CreatedAt);

UPDATE dbo.Requests
SET UpdatedAt = DATEADD(MINUTE, 5, @CreatedAt),
    ApprovedRejectedBy = N'lead@example.com',
    ApprovedRejectedAt = DATEADD(MINUTE, 5, @CreatedAt)
WHERE Id = @ApprovedRequestId;

INSERT INTO dbo.RequestStatusHistory (Id, RequestId, FromStatus, ToStatus, ChangedBy, ChangedAt, Comment)
VALUES
    (NEWID(), @PendingRequestId, NULL, N'Pending', N'alex@example.com', @CreatedAt, N'Request created'),
    (NEWID(), @ApprovedRequestId, NULL, N'Pending', N'sam@example.com', @CreatedAt, N'Request created'),
    (NEWID(), @ApprovedRequestId, N'Pending', N'Approved', N'lead@example.com', DATEADD(MINUTE, 5, @CreatedAt), N'Request approved');
GO
