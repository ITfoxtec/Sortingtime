CREATE TABLE [dbo].[HourPriceSettings] (
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [GroupReferenceKey] NVARCHAR (200)  NULL,
    [HourPrice]         DECIMAL (18, 2) NOT NULL,
    [PartitionId]       BIGINT          NOT NULL,
    [TaskReferenceKey]  NVARCHAR (200)  NOT NULL,
    [Timestamp]         DATETIME2 (7)   NOT NULL,
    [UserId]            BIGINT          NOT NULL,
    CONSTRAINT [PK_HourPriceSettings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HourPriceSettings_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_HourPriceSettings_Partitions_PartitionId] FOREIGN KEY ([PartitionId]) REFERENCES [dbo].[Partitions] ([Id]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_HourPriceSettings_UserId]
    ON [dbo].[HourPriceSettings]([UserId] ASC);


GO



GO
CREATE NONCLUSTERED INDEX [IX_HourPriceSettings_PartitionId_UserId_GroupReferenceKey_TaskReferenceKey]
    ON [dbo].[HourPriceSettings]([PartitionId] ASC, [UserId] ASC, [GroupReferenceKey] ASC, [TaskReferenceKey] ASC);

