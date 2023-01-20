namespace Common.Extensions;

public static class TaskExtensions
{
    public static IEnumerable<Task<T>> OrderByCompletion<T>(this IEnumerable<Task<T>> tasks)
    {
        var lastIndex = -1;

        var tasksArr = tasks.ToList();
        var completionSources = tasksArr.Select(_ => new TaskCompletionSource<T>()).ToArray();

        tasksArr.ForEach(task => task.ContinueWith(Continuation));

        return completionSources.Select(cts => cts.Task);

        void Continuation(Task<T> task)
        {
            var index = Interlocked.Increment(ref lastIndex);
            completionSources![index].TryCompleteFromCompletedTask(task);
        }
    }

    private static bool TryCompleteFromCompletedTask<T>(this TaskCompletionSource<T> tcs, Task<T> task)
    {
        if (task.IsFaulted)
        {
            return tcs.TrySetException(task.Exception!.InnerExceptions);
        }

        if (task.IsCanceled)
        {
            return tcs.TrySetCanceled();
        }

        // Тут уже завершенная задача, блокировки не будет
        return tcs.TrySetResult(task.Result);
    }
}
