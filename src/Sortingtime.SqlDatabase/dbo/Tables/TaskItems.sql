CREATE TABLE [dbo].[TaskItems] (
    [Id]     BIGINT   IDENTITY (1, 1) NOT NULL,
    [Date]   DATE     NOT NULL,
    [TaskId] BIGINT   NOT NULL,
    [Time]   SMALLINT NOT NULL,
    [UserId] BIGINT   NOT NULL,
    CONSTRAINT [PK_TaskItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TaskItems_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TaskItems_Tasks_TaskId] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TaskItems_UserId_Date]
    ON [dbo].[TaskItems]([UserId] ASC, [Date] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TaskItems_TaskId]
    ON [dbo].[TaskItems]([TaskId] ASC);

