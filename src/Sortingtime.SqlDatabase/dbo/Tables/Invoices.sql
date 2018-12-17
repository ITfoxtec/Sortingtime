CREATE TABLE [dbo].[Invoices] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [CustomerShort]   NVARCHAR (MAX)  NULL,
    [EmailBody]       NVARCHAR (4000) NULL,
    [EmailSubject]    NVARCHAR (400)  NULL,
    [FromEmail]       NVARCHAR (200)  NULL,
    [FromFullName]    NVARCHAR (200)  NULL,
    [InvoiceData]     NVARCHAR (MAX)  NULL,
    [InvoiceDate]     DATE            NOT NULL,
    [Number]          BIGINT          NOT NULL,
    [PartitionId]     BIGINT          NOT NULL,
    [Status]          INT             NOT NULL,
    [SubTotalPrice]   DECIMAL (18, 2) NOT NULL,
    [Timestamp]       DATETIME2 (7)   NOT NULL,
    [ToEmail]         NVARCHAR (400)  NULL,
    [UpdateTimestamp] DATETIME2 (7)   NOT NULL,
    [UserId]          BIGINT          NOT NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Invoices_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Invoices_Partitions_PartitionId] FOREIGN KEY ([PartitionId]) REFERENCES [dbo].[Partitions] ([Id]) ON DELETE CASCADE
);










GO
CREATE NONCLUSTERED INDEX [IX_Invoices_UserId]
    ON [dbo].[Invoices]([UserId] ASC);


GO



GO
CREATE NONCLUSTERED INDEX [IX_Invoices_PartitionId_Status_InvoiceDate]
    ON [dbo].[Invoices]([PartitionId] ASC, [Status] ASC, [InvoiceDate] ASC);

