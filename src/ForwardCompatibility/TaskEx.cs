namespace LostTech.Storage.ForwardCompatibility
{
    using System;
    using System.Threading.Tasks;

    static class TaskEx
    {
        public static Task<T> FromException<T>(Exception exception)
        {
            var result = new TaskCompletionSource<T>();
            result.TrySetException(exception);
            return result.Task;
        }
    }
}
