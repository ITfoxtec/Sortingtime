CREATE TABLE [dbo].[MaterialItems] (
    [Id]         BIGINT IDENTITY (1, 1) NOT NULL,
    [Date]       DATE   NOT NULL,
    [MaterialId] BIGINT NOT NULL,
    [Quantity]   INT    NOT NULL,
    [UserId]     BIGINT NOT NULL,
    CONSTRAINT [PK_MaterialItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MaterialItems_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MaterialItems_Materials_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Materials] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MaterialItems_UserId_Date]
    ON [dbo].[MaterialItems]([UserId] ASC, [Date] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MaterialItems_MaterialId]
    ON [dbo].[MaterialItems]([MaterialId] ASC);

