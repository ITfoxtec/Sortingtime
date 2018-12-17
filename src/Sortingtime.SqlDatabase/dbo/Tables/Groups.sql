CREATE TABLE [dbo].[Groups] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (200) NOT NULL,
    [PartitionId] BIGINT         NOT NULL,
    CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Groups_Partitions_PartitionId] FOREIGN KEY ([PartitionId]) REFERENCES [dbo].[Partitions] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_Groups_PartitionId_Name]
    ON [dbo].[Groups]([PartitionId] ASC, [Name] ASC);

