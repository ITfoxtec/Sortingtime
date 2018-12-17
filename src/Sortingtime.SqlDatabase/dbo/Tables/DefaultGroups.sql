CREATE TABLE [dbo].[DefaultGroups] (
    [Id]      BIGINT IDENTITY (1, 1) NOT NULL,
    [GroupId] BIGINT NOT NULL,
    [UserId]  BIGINT NOT NULL,
    CONSTRAINT [PK_DefaultGroups] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DefaultGroups_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DefaultGroups_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_DefaultGroups_UserId]
    ON [dbo].[DefaultGroups]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DefaultGroups_GroupId]
    ON [dbo].[DefaultGroups]([GroupId] ASC);

