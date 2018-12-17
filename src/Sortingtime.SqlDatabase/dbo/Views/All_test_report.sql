


CREATE VIEW [dbo].[All_test_report]
AS
SELECT        TOP (1000) dbo.Partitions.Id AS PartitionId, dbo.Partitions.[Plan], dbo.Organizations.Name AS OrganizationName, dbo.Reports.Timestamp, dbo.Reports.TotalTime, dbo.AspNetUsers.Email
FROM            dbo.Partitions INNER JOIN
                         dbo.Organizations ON dbo.Partitions.Id = dbo.Organizations.Id INNER JOIN
                         dbo.Reports ON dbo.Partitions.Id = dbo.Reports.PartitionId INNER JOIN
                         dbo.AspNetUsers ON dbo.Reports.UserId = dbo.AspNetUsers.Id
ORDER BY PartitionId desc, dbo.Reports.Timestamp desc