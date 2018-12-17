CREATE TABLE [dbo].[DefaultTasks] (
    [Id]     BIGINT IDENTITY (1, 1) NOT NULL,
    [TaskId] BIGINT NOT NULL,
    [UserId] BIGINT NOT NULL,
    CONSTRAINT [PK_DefaultTasks] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DefaultTasks_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DefaultTasks_Tasks_TaskId] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_DefaultTasks_UserId]
    ON [dbo].[DefaultTasks]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DefaultTasks_TaskId]
    ON [dbo].[DefaultTasks]([TaskId] ASC);

