

CREATE VIEW [dbo].[All_by_date_test_time]
AS
SELECT        TOP (1000) dbo.Partitions.Id AS PartitionId, dbo.Partitions.[Plan], dbo.Organizations.Name AS OrganizationName, dbo.GroupTasks.[Group], dbo.GroupTasks.Task, dbo.Works.Date, dbo.Works.Time, 
                         dbo.AspNetUsers.Email
FROM            dbo.GroupTasks INNER JOIN
                         dbo.Works ON dbo.GroupTasks.Id = dbo.Works.GroupTaskId INNER JOIN
                         dbo.Partitions ON dbo.GroupTasks.PartitionId = dbo.Partitions.Id INNER JOIN
                         dbo.Organizations ON dbo.Partitions.Id = dbo.Organizations.Id INNER JOIN
                         dbo.AspNetUsers ON dbo.Works.UserId = dbo.AspNetUsers.Id
ORDER BY dbo.Works.Date desc, PartitionId desc