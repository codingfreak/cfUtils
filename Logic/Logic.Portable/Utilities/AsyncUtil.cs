namespace codingfreaks.cfUtils.Logic.Portable.Utilities
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains wrappers and helpers for async method access.
    /// </summary>
    public static class AsyncUtil
    {
        #region methods

        /// <summary>
        /// Calls an async <paramref name="method"/> and waits for execution so that the call is working like a synchronous one.
        /// </summary>
        /// <remarks>The target async method will be invoked without task context restoring!</remarks>
        /// <typeparam name="TResult">The exprected result type.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <returns>The result of the execution.</returns>
        public static TResult CallSync<TResult>(Func<Task<TResult>> method)
        {            
            var result = default(TResult);
            method().ContinueWith(t => result = t.Result).Wait();
            return result;
        }

        /// <summary>
        /// Calls an async <paramref name="method"/> and waits for execution so that the call is working like a synchronous one.
        /// </summary>
        /// <remarks>The target async method will be invoked without task context restoring!</remarks>
        /// <typeparam name="TResult">The exprected result type.</typeparam>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <param name="arg1">The value for the first parameter.</param>
        /// <returns>The result of the execution.</returns>
        public static TResult CallSync<TResult, T1>(Func<T1, Task<TResult>> method, T1 arg1)
        {
            var result = default(TResult);
            method(arg1).ContinueWith(t => result = t.Result).Wait();
            return result;
        }

        /// <summary>
        /// Calls an async <paramref name="method"/> and waits for execution so that the call is working like a synchronous one.
        /// </summary>
        /// <typeparam name="TResult">The exprected result type.</typeparam>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <param name="arg1">The value for the first parameter.</param>
        /// <param name="arg2">The value for the second parameter.</param>
        /// <returns>The result of the execution.</returns>
        public static TResult CallSync<TResult, T1, T2>(Func<T1, T2, Task<TResult>> method, T1 arg1, T2 arg2)
        {
            var result = default(TResult);
            method(arg1, arg2).ContinueWith(t => result = t.Result).Wait();
            return result;
        }

        /// <summary>
        /// Calls an async <paramref name="method"/> and waits for execution so that the call is working like a synchronous one.
        /// </summary>
        /// <typeparam name="TResult">The exprected result type.</typeparam>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <param name="arg1">The value for the first parameter.</param>
        /// <param name="arg2">The value for the second parameter.</param>
        /// <param name="arg3">The value for the third parameter.</param>
        /// <returns>The result of the execution.</returns>
        public static TResult CallSync<TResult, T1, T2, T3>(Func<T1, T2, T3, Task<TResult>> method, T1 arg1, T2 arg2, T3 arg3)
        {
            var result = default(TResult);
            method(arg1, arg2, arg3).ContinueWith(t => result = t.Result).Wait();
            return result;
        }

        /// <summary>
        /// Calls an async <paramref name="method"/> and waits for execution so that the call is working like a synchronous one.
        /// </summary>
        /// <typeparam name="TResult">The exprected result type.</typeparam>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <param name="arg1">The value for the first parameter.</param>
        /// <param name="arg2">The value for the second parameter.</param>
        /// <param name="arg3">The value for the third parameter.</param>
        /// <param name="arg4">The value for the fourth parameter.</param>
        /// <returns>The result of the execution.</returns>
        public static TResult CallSync<TResult, T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task<TResult>> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var result = default(TResult);
            method(arg1, arg2, arg3, arg4).ContinueWith(t => result = t.Result).Wait();
            return result;
        }

        /// <summary>
        /// Calls an async <paramref name="method"/> and waits for execution so that the call is working like a synchronous one.
        /// </summary>
        /// <typeparam name="TResult">The exprected result type.</typeparam>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <param name="arg1">The value for the first parameter.</param>
        /// <param name="arg2">The value for the second parameter.</param>
        /// <param name="arg3">The value for the third parameter.</param>
        /// <param name="arg4">The value for the fourth parameter.</param>
        /// <param name="arg5">The value for the fifth parameter.</param>
        /// <returns>The result of the execution.</returns>
        public static TResult CallSync<TResult, T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, Task<TResult>> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var result = default(TResult);
            method(arg1, arg2, arg3, arg4, arg5).ContinueWith(t => result = t.Result).Wait();
            return result;
        }

        #endregion
    }
}