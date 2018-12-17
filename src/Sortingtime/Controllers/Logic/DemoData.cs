using Sortingtime.Infrastructure;
using Sortingtime.Models;
using System;
using System.Threading.Tasks;
using Sortingtime.Infrastructure.Translation;

namespace Sortingtime.Controllers.Logic
{
    public class DemoData
    {
        private readonly ApplicationDbContext dbContext;
        private readonly Translate translate;

        public DemoData(ApplicationDbContext dbContext, Translate translate)
        {
            this.dbContext = dbContext;
            this.translate = translate;
        }

        public async Task<long> CreateDemo(string remoteIpAddress)
        {
            var demo = Demo.CreateNew();
            demo.RemoteIpAddress = remoteIpAddress?.MaxLength(200);
            dbContext.Demos.Add(demo);
            await dbContext.SaveChangesAsync();
            return demo.Id + 453472;
        }

        public void AddDemoData(long partitionId, ApplicationUser user1, ApplicationUser user2)
        {
            var date = DateTime.Now.Date;

            var g1 = new Group { PartitionId = partitionId, Name = translate.Get("DEMO.DEMO_GROUP")};
            var g2 = new Group { PartitionId = partitionId, Name = translate.Get("DEMO.ANOTHER_DEMO_GROUP")};
            //dbContext.Groups.Add(g1);
            //dbContext.Groups.Add(g2);

            var t1 = new Ttask { Name = translate.Get("DEMO.SOME_DEMO_TASK"), Group = g1 };
            var t2 = new Ttask { Name = translate.Get("DEMO.ANOTHER_DEMO_TASK"), Group = g1 };
            var t3 = new Ttask { Name = translate.Get("DEMO.YET_ANOTHER_DEMO_TASK"), Group = g2 };
            //dbContext.Tasks.Add(t1);
            //dbContext.Tasks.Add(t2);
            //dbContext.Tasks.Add(t3);

            dbContext.DefaultTasks.Add(new DefaultTask { Task = t1, User = user1 });
            dbContext.DefaultTasks.Add(new DefaultTask { Task = t2, User = user1 });
            dbContext.DefaultTasks.Add(new DefaultTask { Task = t3, User = user1 });

            AddWork1(date, user1, t1, t2, t3);
            AddWork2(date, user2, t2, t3);
        }

        private void AddWork1(DateTime date, ApplicationUser user, Ttask t1, Ttask t2, Ttask t3)
        {
            var dayCurcer = date.AddDays(-7);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t1, Time = 9 * 60 + 10 });
            }
            dayCurcer = date.AddDays(-6);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t1, Time = 2 * 60 + 05 });
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 6 * 60 + 20 });
            }
            dayCurcer = date.AddDays(-5);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t1, Time = 2 * 60 });
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t2, Time = 3 * 60 + 10 });
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 3 * 60 });
            }
            dayCurcer = date.AddDays(-4);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 2 * 60 });
            }
            dayCurcer = date.AddDays(-3);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t2, Time = 2 * 60 + 40 });
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 4 * 60 });
            }
            dayCurcer = date.AddDays(-2);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 6 * 60 });
            }
            dayCurcer = date.AddDays(-1);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t1, Time = 1 * 60 + 45 });
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t2, Time = 6 * 60 });
            }
            dayCurcer = date;
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t1, Time = 1 * 60 + 10 });
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 5 * 60 });
            }
        }
        private void AddWork2(DateTime date, ApplicationUser user, Ttask t2, Ttask t3)
        {
            var dayCurcer = date.AddDays(-7);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t2, Time = 5 * 60 + 30 });
            }
            dayCurcer = date.AddDays(-6);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t2, Time = 10 * 60 });
            }
            dayCurcer = date.AddDays(-5);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t2, Time = 1 * 60 + 10 });
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 5 * 60 });
            }
            dayCurcer = date.AddDays(-4);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 4 * 60 + 15 });
            }
            dayCurcer = date.AddDays(-3);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 9 * 60 });
            }
            dayCurcer = date.AddDays(-2);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 8 * 60 + 20 });
            }
            dayCurcer = date.AddDays(-1);
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t2, Time = 6 * 60 });
            }
            dayCurcer = date;
            if (IsNotWeekend(dayCurcer))
            {
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t2, Time = 2 * 60 });
                dbContext.TaskItems.Add(new TtaskItem { Date = dayCurcer, User = user, Task = t3, Time = 3 * 60 + 40 });
            }
        }
        
        private bool IsNotWeekend(DateTime dayCurcer)
        {
            if (dayCurcer.DayOfWeek == DayOfWeek.Saturday || dayCurcer.DayOfWeek == DayOfWeek.Sunday)
                return false;
            else
                return true;
        }
    }
}
