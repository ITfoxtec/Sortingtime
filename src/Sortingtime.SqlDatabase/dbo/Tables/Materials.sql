CREATE TABLE [dbo].[Materials] (
    [Id]      BIGINT          IDENTITY (1, 1) NOT NULL,
    [GroupId] BIGINT          NOT NULL,
    [Name]    NVARCHAR (200)  NOT NULL,
    [Price]   DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_Materials] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Materials_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Materials_GroupId_Name]
    ON [dbo].[Materials]([GroupId] ASC, [Name] ASC);

