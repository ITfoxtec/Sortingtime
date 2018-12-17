

CREATE VIEW [dbo].[All_by_date_test_invoice]
AS
SELECT        TOP (1000) dbo.Partitions.Id AS PartitionId, dbo.Partitions.[Plan], dbo.Organizations.Name AS OrganizationName, dbo.Invoices.Timestamp, dbo.Invoices.SubTotalPrice, dbo.AspNetUsers.Email
FROM            dbo.Partitions INNER JOIN
                         dbo.Organizations ON dbo.Partitions.Id = dbo.Organizations.Id INNER JOIN
                         dbo.Invoices ON dbo.Partitions.Id = dbo.Invoices.PartitionId INNER JOIN
                         dbo.AspNetUsers ON dbo.Invoices.UserId = dbo.AspNetUsers.Id
ORDER BY dbo.Invoices.Timestamp desc, PartitionId desc