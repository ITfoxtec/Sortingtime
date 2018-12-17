CREATE TABLE [dbo].[DefaultMaterials] (
    [Id]         BIGINT IDENTITY (1, 1) NOT NULL,
    [MaterialId] BIGINT NOT NULL,
    [UserId]     BIGINT NOT NULL,
    CONSTRAINT [PK_DefaultMaterials] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DefaultMaterials_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DefaultMaterials_Materials_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Materials] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_DefaultMaterials_UserId]
    ON [dbo].[DefaultMaterials]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DefaultMaterials_MaterialId]
    ON [dbo].[DefaultMaterials]([MaterialId] ASC);

