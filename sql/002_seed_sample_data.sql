USE ReqFlow;
GO

DECLARE @PendingRequestId UNIQUEIDENTIFIER = NEWID();
DECLARE @ApprovedRequestId UNIQUEIDENTIFIER = NEWID();
DECLARE @AlexUserId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000001';
DECLARE @SamUserId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000002';
DECLARE @LeadUserId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000003';
DECLARE @AdminUserId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000004';
DECLARE @InactiveUserId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000005';
DECLARE @CreatedAt DATETIME2 = SYSUTCDATETIME();

INSERT INTO dbo.Users (Id, Email, DisplayName, Role, IsActive)
VALUES
    (@AlexUserId, N'alex@example.com', N'Alex Requester', N'Requester', 1),
    (@SamUserId, N'sam@example.com', N'Sam Requester', N'Requester', 1),
    (@LeadUserId, N'lead@example.com', N'Lee Approver', N'Approver', 1),
    (@AdminUserId, N'admin@example.com', N'Casey Admin', N'Admin', 1),
    (@InactiveUserId, N'inactive@example.com', N'Inactive User', N'Approver', 0);

INSERT INTO dbo.Requests (Id, Title, Description, Status, RequestedByUserId, RequestedBy, CreatedAt)
VALUES
    (@PendingRequestId, N'New laptop', N'Replacement laptop for a developer.', N'Pending', @AlexUserId, N'alex@example.com', @CreatedAt),
    (@ApprovedRequestId, N'Training budget', N'Cloud architecture course registration.', N'Approved', @SamUserId, N'sam@example.com', @CreatedAt);

UPDATE dbo.Requests
SET UpdatedAt = DATEADD(MINUTE, 5, @CreatedAt),
    ApprovedRejectedByUserId = @LeadUserId,
    ApprovedRejectedBy = N'lead@example.com',
    ApprovedRejectedAt = DATEADD(MINUTE, 5, @CreatedAt)
WHERE Id = @ApprovedRequestId;

INSERT INTO dbo.RequestStatusHistory (Id, RequestId, ChangedByUserId, FromStatus, ToStatus, ChangedBy, ChangedAt, Comment)
VALUES
    (NEWID(), @PendingRequestId, @AlexUserId, NULL, N'Pending', N'alex@example.com', @CreatedAt, N'Request created'),
    (NEWID(), @ApprovedRequestId, @SamUserId, NULL, N'Pending', N'sam@example.com', @CreatedAt, N'Request created'),
    (NEWID(), @ApprovedRequestId, @LeadUserId, N'Pending', N'Approved', N'lead@example.com', DATEADD(MINUTE, 5, @CreatedAt), N'Request approved');
GO
