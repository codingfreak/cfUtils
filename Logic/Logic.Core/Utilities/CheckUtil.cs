namespace codingfreaks.cfUtils.Logic.Core.Utilities
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides methods for easy access to commonly used runtime checks.
    /// </summary>
    public static class CheckUtil
    {
        #region methods

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> containing the name of the parameter in the <paramref name="expression" />
        /// if it is not
        /// member of the enumeration of type <typeparamref name="TEnum" />.
        /// </summary>
        /// <example>
        /// void Foo(object someData)
        /// {
        ///     CheckUtil.ThrowIfInvalidEnum[MyEnumType](() => myEnumValue);
        /// }
        /// </example>
        /// <typeparam name="TEnum">The type of the enumeration in which the <paramref name="expression" /> must be contained.</typeparam>
        /// <param name="expression">The expression containing the enum-variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the exception throwing.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="expression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Is thrown if the <paramref name="expression"/> retrieves an enum value not defined in <typeparamref name="TEnum"/>.</exception>
        public static void ThrowIfInvalidEnum<TEnum>(Expression<Func<TEnum>> expression, Action<Exception> callbackOnException = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            var enumValue = expression.Compile().Invoke();
            if (Enum.IsDefined(typeof(TEnum), enumValue))
            {
                return;
            }
            var expressionBody = (ConstantExpression)expression.Body;
            var ex = new ArgumentException($"Provided value [{expressionBody}] is not part of the enumeration");
            callbackOnException?.Invoke(ex);
            throw ex;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException" /> containing the name of the parameter in the
        /// <paramref name="expression" /> if it compiles to <c>null</c>.
        /// </summary>
        /// <example>
        /// void Foo(object someData)
        /// {
        ///     CheckUtil.ThrowIfNull(() => someData);
        /// }
        /// </example>
        /// <typeparam name="T">The type of the parameter which will be passed.</typeparam>
        /// <param name="expression">The expression containing the variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the exception throwing.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="expression"/> is <c>null</c>.</exception>
        /// <exception cref="NullReferenceException">Is thrown if the <paramref name="expression"/> retrieves <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "This CA-Warning is simply wrong.")]
        public static void ThrowIfNull<T>(Expression<Func<T>> expression, Action<Exception> callbackOnException = null)
            where T : class
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            if (expression.Compile().Invoke() != null)
            {
                return;
            }
            var message = string.Empty;
            if (expression.Body is MemberExpression memberExpression)
            {
                message = memberExpression.Member.Name;
            }            
            var ex = new NullReferenceException(message);
            callbackOnException?.Invoke(ex);
            throw ex;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> containing the name of the parameter in the
        /// <paramref name="expression" />
        /// if contains null or is equal to <see cref="string.Empty" />.
        /// </summary>
        /// <example>
        /// void Foo(string someData)
        /// {
        ///     CheckUtil.ThrowIfNullOrEmpty(() => someData);
        /// }
        /// </example>
        /// <param name="expression">The expression containing the variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the exception throwing.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="expression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Is thrown if the <paramref name="expression"/> returns an empty string or <c>null</c>.</exception>
        public static void ThrowIfNullOrEmpty(Expression<Func<string>> expression, Action<Exception> callbackOnException = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            if (!string.IsNullOrEmpty(expression.Compile().Invoke()))
            {
                return;
            }
            ArgumentException ex;
            if (expression.Body is MemberExpression memberExpression)
            {
                ex = new ArgumentException("Value can't be null or empty.", memberExpression.Member.Name);
            }
            else
            {
                ex = new ArgumentException("Value can't be null or empty.");
            }
            callbackOnException?.Invoke(ex);
            throw ex;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> containing the name of the parameter in the <paramref name="expression" />
        /// if contains null or just a white space.
        /// </summary>
        /// <example>
        /// void Foo(string someData)
        /// {
        ///     CheckUtil.CheckNullOrWhitespace(() => someData);
        /// }
        /// </example>
        /// <param name="expression">The expression containing the variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the exception throwing.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="expression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Is thrown if the <paramref name="expression"/> returns an empty string, <c>null</c> or just a white space.</exception>
        public static void ThrowIfNullOrWhitespace(Expression<Func<string>> expression, Action<Exception> callbackOnException = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            if (!string.IsNullOrWhiteSpace(expression.Compile().Invoke()))
            {
                return;
            }
            ArgumentException ex;
            if (expression.Body is MemberExpression memberExpression)
            {                
                ex = new ArgumentException("Value can't be null or white space.", memberExpression.Member.Name);
            }
            else
            {
                ex = new ArgumentException("Value can't be null or white space.");
            }            
            callbackOnException?.Invoke(ex);
            throw ex;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> containing the name of the parameter in the <paramref name="expression" />
        /// if contains zero or negative.
        /// </summary>
        /// <example>
        /// void Foo(long someData)
        /// {
        ///     CheckUtil.ThrowIfZeroOrNegative(() => someData);
        /// }
        /// </example>
        /// <param name="expression">The expression containing the variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the exception throwing.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="expression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Is thrown if the <paramref name="expression"/> returns a number smaller or equal to 0.</exception>
        public static void ThrowIfZeroOrNegative(Expression<Func<long>> expression, Action<Exception> callbackOnException = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            if (expression.Compile().Invoke() > 0)
            {
                return;
            }
            ArgumentException ex;
            if (expression.Body is MemberExpression memberExpression)
            {
                ex = new ArgumentException("Value can't be zero or negative.", memberExpression.Member.Name);
            }
            else
            {
                ex = new ArgumentException("Value can't be zero or negative.");
            }            
            callbackOnException?.Invoke(ex);
            throw ex;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> containing the name of the parameter in the <paramref name="expression" />
        /// if contains zero or negative.
        /// </summary>
        /// <example>
        /// void Foo(int someData)
        /// {
        ///     CheckUtil.ThrowIfZeroOrNegative(() => someData);
        /// }
        /// </example>
        /// <param name="expression">The expression containing the variable.</param>
        /// <param name="callbackOnException">An optional callback method that will be called right before the exception throwing.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="expression"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Is thrown if the <paramref name="expression"/> returns a number smaller or equal to 0.</exception>
        public static void ThrowIfZeroOrNegative(Expression<Func<int>> expression, Action<Exception> callbackOnException = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            if (expression.Compile().Invoke() > 0)
            {
                return;
            }
            ArgumentException ex;
            if (expression.Body is MemberExpression memberExpression)
            {
                ex = new ArgumentException("Value can't be zero or negative.", memberExpression.Member.Name);
            }
            else
            {
                ex = new ArgumentException("Value can't be zero or negative.");
            }
            callbackOnException?.Invoke(ex);
            throw ex;
        }

        #endregion
    }
}