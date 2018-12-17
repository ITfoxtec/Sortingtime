CREATE TABLE [dbo].[ReportSettings] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmailBody]     NVARCHAR (4000) NULL,
    [EmailSubject]  NVARCHAR (400)  NULL,
    [PartitionId]   BIGINT          NOT NULL,
    [ReferenceKey]  NVARCHAR (200)  NOT NULL,
    [ReferenceType] INT             NOT NULL,
    [ReportText]    NVARCHAR (4000) NULL,
    [ReportTitle]   NVARCHAR (200)  NULL,
    [ToEmail]       NVARCHAR (400)  NULL,
    CONSTRAINT [PK_ReportSettings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportSettings_Partitions_PartitionId] FOREIGN KEY ([PartitionId]) REFERENCES [dbo].[Partitions] ([Id]) ON DELETE CASCADE
);












GO



GO
CREATE NONCLUSTERED INDEX [IX_ReportSettings_PartitionId_ReferenceType_ReferenceKey]
    ON [dbo].[ReportSettings]([PartitionId] ASC, [ReferenceType] ASC, [ReferenceKey] ASC);

