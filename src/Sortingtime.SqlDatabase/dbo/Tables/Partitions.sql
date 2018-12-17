CREATE TABLE [dbo].[Partitions] (
    [Id]                  BIGINT        IDENTITY (1, 1) NOT NULL,
    [CreatedByUserId]     BIGINT        NOT NULL,
    [MaxInvoicesPerMonth] SMALLINT      NOT NULL,
    [MaxReportsPerMonth]  SMALLINT      NOT NULL,
    [MaxUsers]            SMALLINT      NOT NULL,
    [Plan]                INT           NOT NULL,
    [Timestamp]           DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_Partitions] PRIMARY KEY CLUSTERED ([Id] ASC)
);







