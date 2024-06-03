namespace MultimediaBuilder.Utils
{
    public static class ParallelTasking
    {
        public static void ForeachWithFallback<T>(IEnumerable<T> collection, Action<T> action)
        {
            // Get the number of available worker threads
            ThreadPool.GetAvailableThreads(out int availableWorkerThreads, out int _);

            if (availableWorkerThreads > 1) // Arbitrary threshold to decide if parallelism is feasible
            {
                try
                {
                    Parallel.ForEach(collection, item =>
                    {
                        action(item);
                    });
                }
                catch (AggregateException ex)
                {
                    foreach (var innerException in ex.InnerExceptions)
                    {
                        Console.WriteLine(innerException.Message);
                    }
                }
            }
            else
            {
                // Fall back to sequential processing
                foreach (var item in collection)
                {
                    action(item);
                }
            }
        }
    }
}