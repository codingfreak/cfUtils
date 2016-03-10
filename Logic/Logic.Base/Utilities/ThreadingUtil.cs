namespace codingfreaks.cfUtils.Logic.Base.Utilities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods useful when dealing with multiple threads.
    /// </summary>
    public static class ThreadingUtil
    {
        #region methods

        /// <summary>
        /// Performs an <paramref name="action"/> wrapping it inside a <paramref name="threadLock"/> and repeats this when an exception happens.
        /// </summary>
        /// <remarks>
        /// This can be useful when handling with non-thread-safe resources like lists e.g.
        /// </remarks>
        /// <param name="action">The action to perform which is suspicious in terms of thread-safety.</param>
        /// <param name="threadLock">A lock object to use for wrapping the <paramref name="action"/>.</param>
        /// <param name="maxRetryCount">The maximum amount of retries to allow.</param>
        /// <param name="millisecondsBetweenRetries">The time in millisesonds to wait between each retry-step.</param>
        /// <param name="increaseWaitTime">If set to <c>true</c> the <paramref name="increaseWaitTime"/> will be increased at each iteration.</param>
        /// <returns><c>true</c> if the <paramref name="action"/> could be performed.</returns>
        public static bool PerformWithLock(Action action, object threadLock, int maxRetryCount = 5, int millisecondsBetweenRetries = 100, bool increaseWaitTime = true)
        {
            var retries = maxRetryCount;
            var ok = true;
            while (retries > 0)
            {
                retries--;
                try
                {
                    lock (threadLock)
                    {
                        action.Invoke();
                    }
                }
                catch
                {
                    ok = false;
                }
                if (ok)
                {
                    break;
                }
                var waitTime = millisecondsBetweenRetries;
                if (increaseWaitTime)
                {
                    waitTime = (maxRetryCount - retries + 1) * millisecondsBetweenRetries;
                }
                Task.Delay(waitTime).Wait();
            }
            return ok;
        }

        #endregion
    }
}