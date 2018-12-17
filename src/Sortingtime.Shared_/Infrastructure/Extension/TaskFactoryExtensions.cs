using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sortingtime.Infrastructure
{
    public static class TaskFactoryExtensions
    {
        public static Task<TResult> StartNewPersistedCulture<TResult>(this TaskFactory taskFactory, 
            Func<TResult> function, 
            CancellationToken cancellationToken = default(CancellationToken), 
            TaskCreationOptions creationOptions = default(TaskCreationOptions))
        {
            if (taskFactory == null) throw new ArgumentNullException(nameof(taskFactory));
            if (function == null) throw new ArgumentNullException(nameof(function));

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var currentUICulture = Thread.CurrentThread.CurrentUICulture;
            return taskFactory.StartNew(
                () =>
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUICulture;

                    return function();
                }, cancellationToken, creationOptions, TaskScheduler.Default);
        }

        public static Task StartNewPersistedCulture(this TaskFactory taskFactory, 
            Action action, 
            CancellationToken cancellationToken = default(CancellationToken), 
            TaskCreationOptions creationOptions = default(TaskCreationOptions))
        {
            if (taskFactory == null) throw new ArgumentNullException(nameof(taskFactory));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var currentUICulture = Thread.CurrentThread.CurrentUICulture;
            return taskFactory.StartNew(
                () =>
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUICulture;

                    action();
                }, cancellationToken, creationOptions, TaskScheduler.Default);
        }
    }
}
