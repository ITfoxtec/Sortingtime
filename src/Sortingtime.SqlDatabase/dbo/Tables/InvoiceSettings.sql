CREATE TABLE [dbo].[InvoiceSettings] (
    [Id]                  BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmailBody]           NVARCHAR (4000) NULL,
    [EmailSubject]        NVARCHAR (400)  NULL,
    [InvoiceCustomer]     NVARCHAR (400)  NULL,
    [InvoicePaymentTerms] NVARCHAR (400)  NULL,
    [InvoiceReference]    NVARCHAR (100)  NULL,
    [InvoiceText]         NVARCHAR (4000) NULL,
    [InvoiceTitle]        NVARCHAR (200)  NULL,
    [PartitionId]         BIGINT          NOT NULL,
    [ReferenceKey]        NVARCHAR (200)  NOT NULL,
    [ReferenceType]       INT             NOT NULL,
    [ToEmail]             NVARCHAR (400)  NULL,
    CONSTRAINT [PK_InvoiceSettings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InvoiceSettings_Partitions_PartitionId] FOREIGN KEY ([PartitionId]) REFERENCES [dbo].[Partitions] ([Id]) ON DELETE CASCADE
);












GO



GO
CREATE NONCLUSTERED INDEX [IX_InvoiceSettings_PartitionId_ReferenceType_ReferenceKey]
    ON [dbo].[InvoiceSettings]([PartitionId] ASC, [ReferenceType] ASC, [ReferenceKey] ASC);

