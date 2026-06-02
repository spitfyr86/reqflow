IF DB_ID(N'ReqFlow') IS NULL
BEGIN
    CREATE DATABASE ReqFlow;
END;
GO

USE ReqFlow;
GO

IF OBJECT_ID(N'dbo.RequestStatusHistory', N'U') IS NOT NULL DROP TABLE dbo.RequestStatusHistory;
IF OBJECT_ID(N'dbo.Requests', N'U') IS NOT NULL DROP TABLE dbo.Requests;
GO

CREATE TABLE dbo.Requests
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Requests PRIMARY KEY,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    RequestedBy NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    ApprovedRejectedBy NVARCHAR(100) NULL,
    ApprovedRejectedAt DATETIME2 NULL,
    RejectionReason NVARCHAR(500) NULL,
    RowVersion ROWVERSION NOT NULL,
    CONSTRAINT CK_Requests_Status CHECK (Status IN (N'Pending', N'Approved', N'Rejected'))
);
GO

CREATE TABLE dbo.RequestStatusHistory
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_RequestStatusHistory PRIMARY KEY,
    RequestId UNIQUEIDENTIFIER NOT NULL,
    FromStatus NVARCHAR(30) NULL,
    ToStatus NVARCHAR(30) NOT NULL,
    ChangedBy NVARCHAR(100) NOT NULL,
    ChangedAt DATETIME2 NOT NULL,
    Comment NVARCHAR(500) NULL,
    CONSTRAINT FK_RequestStatusHistory_Requests FOREIGN KEY (RequestId) REFERENCES dbo.Requests(Id),
    CONSTRAINT CK_RequestStatusHistory_FromStatus CHECK (FromStatus IS NULL OR FromStatus IN (N'Pending', N'Approved', N'Rejected')),
    CONSTRAINT CK_RequestStatusHistory_ToStatus CHECK (ToStatus IN (N'Pending', N'Approved', N'Rejected'))
);
GO

CREATE INDEX IX_Requests_Status ON dbo.Requests(Status);
CREATE INDEX IX_Requests_CreatedAt ON dbo.Requests(CreatedAt);
CREATE INDEX IX_RequestStatusHistory_RequestId ON dbo.RequestStatusHistory(RequestId);
GO
