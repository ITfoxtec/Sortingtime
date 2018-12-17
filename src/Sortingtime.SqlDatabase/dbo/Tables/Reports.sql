CREATE TABLE [dbo].[Reports] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmailBody]       NVARCHAR (4000) NULL,
    [EmailSubject]    NVARCHAR (400)  NULL,
    [FromDate]        DATE            NOT NULL,
    [FromEmail]       NVARCHAR (200)  NULL,
    [FromFullName]    NVARCHAR (200)  NULL,
    [Number]          BIGINT          NOT NULL,
    [PartitionId]     BIGINT          NOT NULL,
    [ReportData]      NVARCHAR (MAX)  NULL,
    [Status]          INT             NOT NULL,
    [Timestamp]       DATETIME2 (7)   NOT NULL,
    [ToDate]          DATE            NOT NULL,
    [ToEmail]         NVARCHAR (400)  NULL,
    [TotalTime]       INT             NOT NULL,
    [UpdateTimestamp] DATETIME2 (7)   NOT NULL,
    [UserId]          BIGINT          NOT NULL,
    CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Reports_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reports_Partitions_PartitionId] FOREIGN KEY ([PartitionId]) REFERENCES [dbo].[Partitions] ([Id]) ON DELETE CASCADE
);












GO
CREATE NONCLUSTERED INDEX [IX_Reports_UserId]
    ON [dbo].[Reports]([UserId] ASC);


GO



GO
CREATE NONCLUSTERED INDEX [IX_Reports_PartitionId_Status_FromDate_ToDate]
    ON [dbo].[Reports]([PartitionId] ASC, [Status] ASC, [FromDate] ASC, [ToDate] ASC);

