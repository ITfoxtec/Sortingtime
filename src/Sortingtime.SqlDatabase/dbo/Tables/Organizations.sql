CREATE TABLE [dbo].[Organizations] (
    [Id]                 BIGINT         NOT NULL,
    [Address]            NVARCHAR (400) NULL,
    [Culture]            NVARCHAR (10)  NOT NULL,
    [FirstInvoiceNumber] INT            NULL,
    [Logo]               NVARCHAR (MAX) NULL,
    [Name]               NVARCHAR (200) NULL,
    [PaymentDetails]     NVARCHAR (400) NULL,
    [TaxPercentage]      INT            NULL,
    [VatNumber]          NVARCHAR (50)  NULL,
    [VatPercentage]      INT            NULL,
    CONSTRAINT [PK_Organizations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Organizations_Partitions_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[Partitions] ([Id]) ON DELETE CASCADE
);














GO




