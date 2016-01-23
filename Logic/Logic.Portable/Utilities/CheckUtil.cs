namespace s2.s2Utils.Logic.Portable.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides methods for easy access to commonly used runtime checks.
    /// </summary>
    public static class CheckUtil
    {
        #region methods

        /// <summary>
        /// Tries to get a string-value out of a dictionary and throws exceptions if the <paramref name="key"/> is not found or the result is null or empty.
        /// </summary>
        /// <param name="targetDictionary">The dictionary to use.</param>
        /// <param name="key">The key inside the <paramref name="targetDictionary"/> keys.</param>
        /// <param name="targetVariable">The variable reference where to store the result in.</param>
        /// <exception cref="KeyNotFoundException">Is thrown if the <paramref name="key"/> is not found in <paramref name="targetDictionary"/>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Won't work without out-parameter.")]
        public static void ReadDictionaryOrThrow(Dictionary<string, string> targetDictionary, string key, out string targetVariable)
        {
            ThrowIfNull(() => targetDictionary);
            if (!targetDictionary.TryGetValue(key, out targetVariable))
            {
                throw new KeyNotFoundException("Dictionary does not contain provided key.");
            }
            if (string.IsNullOrEmpty(targetVariable))
            {
                throw new InvalidOperationException("Invalid value for provided key in dictionary.");
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> containing the name of the parameter in the <paramref name="expression"/> if it is not
        /// member of the enumeration of type <typeparamref name="TEnum"/>.
        /// </summary>
        /// <example>        
        /// void Foo(object someData)
        /// {
        ///     CheckUtil.ThrowIfInvalidEnum[MyEnumType](() => myEnumValue);
        /// }        
        /// </example>
        /// <typeparam name="TEnum">The type of the enumeration in which the <paramref name="expression"/> must be contained.</typeparam>
        /// <param name="expression">The expression containing the enum-variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the expection throwing.</param>
        public static void ThrowIfInvalidEnum<TEnum>(Expression<Func<TEnum>> expression, Action<Exception> callbackOnException = null)
        {
            if (expression == null)
            {
                throw new InvalidOperationException("Expression can't be null.");
            }
            var enumValue = expression.Compile().Invoke();
            if (!Enum.IsDefined(typeof(TEnum), enumValue))
            {
                var expressionBody = (MemberExpression)expression.Body;
                var ex = new ArgumentException("Provided value is not part of the enumaration", expressionBody.Member.Name);
                if (callbackOnException != null)
                {
                    callbackOnException(ex);
                }
                throw ex;
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> containing the name of the parameter in the <paramref name="expression"/> if it compiles to <c>null</c>.
        /// </summary>
        /// <example>
        /// void Foo(object someData)
        /// {
        ///     CheckUtil.ThrowIfNull(() => someData);
        /// }
        /// </example>
        /// <typeparam name="T">The type of the parameter which will be passed.</typeparam>
        /// <param name="expression">The expression containing the variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the expection throwing.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "This CA-Warning is simply wrong.")]
        public static void ThrowIfNull<T>(Expression<Func<T>> expression, Action<Exception> callbackOnException = null) where T : class
        {
            if (expression == null)
            {
                throw new InvalidOperationException("Expression can't be null.");
            }
            if (expression.Compile().Invoke() == null)
            {
                var expressionBody = (MemberExpression)expression.Body;
                var ex = new ArgumentNullException(expressionBody.Member.Name);
                if (callbackOnException != null)
                {
                    callbackOnException(ex);
                }
                throw ex;
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> containing the name of the parameter in the <paramref name="expression"/> if contains null or is equal to <see cref="string.Empty"/>.
        /// </summary>
        /// <example>
        /// void Foo(object someData)
        /// {
        ///     CheckUtil.ThrowIfNullOrEmpty(() => someData);
        /// }
        /// </example>
        /// <param name="expression">The expression containing the variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the expection throwing.</param>
        public static void ThrowIfNullOrEmpty(Expression<Func<string>> expression, Action<Exception> callbackOnException = null)
        {
            if (expression == null)
            {
                throw new InvalidOperationException("Expression can't be null.");
            }
            if (string.IsNullOrEmpty(expression.Compile().Invoke()))
            {
                var expressionBody = (MemberExpression)expression.Body;
                var ex = new ArgumentException("Value can't be null or empty.", expressionBody.Member.Name);
                if (callbackOnException != null)
                {
                    callbackOnException(ex);
                }
                throw ex;
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> containing the name of the parameter in the <paramref name="expression"/> if contains null or just a white space.
        /// </summary>
        /// <example>
        /// void Foo(object someData)
        /// {
        ///     CheckUtil.CheckNullOrWhitespace(() => someData);
        /// }
        /// </example>
        /// <param name="expression">The expression containing the variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the expection throwing.</param>
        public static void ThrowIfNullOrWhitespace(Expression<Func<string>> expression, Action<Exception> callbackOnException = null)
        {
            if (expression == null)
            {
                throw new InvalidOperationException("Expression can't be null.");
            }
            if (string.IsNullOrWhiteSpace(expression.Compile().Invoke()))
            {
                var expressionBody = (MemberExpression)expression.Body;
                var ex = new ArgumentException("Value can't be null or white space.", expressionBody.Member.Name);
                if (callbackOnException != null)
                {
                    callbackOnException(ex);
                }
                throw ex;
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> containing the name of the parameter in the <paramref name="expression"/> if contains zero or negativ.
        /// </summary>
        /// <example>
        /// void Foo(object someData)
        /// {
        ///     CheckUtil.ThrowIfZeroOrNegativ(() => someData);
        /// }
        /// </example>
        /// <param name="expression">The expression containing the variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the expection throwing.</param>
        public static void ThrowIfZeroOrNegativ(Expression<Func<long>> expression, Action<Exception> callbackOnException = null)
        {
            if (expression == null)
            {
                throw new InvalidOperationException("Expression can't be null.");
            }
            if (expression.Compile().Invoke() <= 0)
            {
                var expressionBody = (MemberExpression)expression.Body;
                var ex = new ArgumentException("Value can't be zero or negativ.", expressionBody.Member.Name);
                if (callbackOnException != null)
                {
                    callbackOnException(ex);
                }
                throw ex;
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> containing the name of the parameter in the <paramref name="expression"/> if contains zero or negativ.
        /// </summary>
        /// <example>
        /// void Foo(object someData)
        /// {
        ///     CheckUtil.ThrowIfZeroOrNegativ(() => someData);
        /// }
        /// </example>
        /// <param name="expression">The expression containing the variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the expection throwing.</param>
        public static void ThrowIfZeroOrNegativ(Expression<Func<int>> expression, Action<Exception> callbackOnException = null)
        {
            if (expression == null)
            {
                throw new InvalidOperationException("Expression can't be null.");
            }
            if (expression.Compile().Invoke() <= 0)
            {
                var expressionBody = (MemberExpression)expression.Body;
                var ex = new ArgumentException("Value can't be zero or negativ.", expressionBody.Member.Name);
                if (callbackOnException != null)
                {
                    callbackOnException(ex);
                }
                throw ex;
            }
        }

        #endregion
    }
}