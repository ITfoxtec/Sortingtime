CREATE TABLE [dbo].[Demos] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [RemoteIpAddress] NVARCHAR (200) NULL,
    [Timestamp]       DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Demos] PRIMARY KEY CLUSTERED ([Id] ASC)
);





