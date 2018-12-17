CREATE TABLE [dbo].[Tasks] (
    [Id]      BIGINT         IDENTITY (1, 1) NOT NULL,
    [GroupId] BIGINT         NOT NULL,
    [Name]    NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Tasks_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Tasks_GroupId_Name]
    ON [dbo].[Tasks]([GroupId] ASC, [Name] ASC);

