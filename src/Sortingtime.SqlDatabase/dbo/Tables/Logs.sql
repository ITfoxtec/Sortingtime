CREATE TABLE [dbo].[Logs] (
    [Id]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [LogSource] NVARCHAR (200) NULL,
    [Message]   NVARCHAR (MAX) NULL,
    [Severity]  INT            NOT NULL,
    [Timestamp] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ([Id] ASC)
);





