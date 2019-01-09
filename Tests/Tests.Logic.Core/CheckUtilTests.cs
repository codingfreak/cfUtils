namespace codingfreaks.cfUtils.Tests.Logic.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection.Metadata.Ecma335;

    using cfUtils.Logic.Core.Enumerations;
    using cfUtils.Logic.Core.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using NAssert = NUnit.Framework.Assert;

    /// <summary>
    /// Contains unit tests for <see cref="CheckUtil" />.
    /// </summary>
    [TestClass]
    public class CheckUtilTests
    {
        #region methods

        /// <summary>
        /// Tests the correct functionality of <see cref="CheckUtil.ThrowIfInvalidEnum{TEnum}" />.
        /// </summary>
        [TestMethod]
        public void ThrowIfInvalidEnumTest()
        {
            // arrange
            var values = new List<(Expression<Func<PortStateEnum>> expression, bool shouldRaceNull, bool shouldRaceArgument)>
            {
                (null, true, false),
                (() => (PortStateEnum)10, false, true),
                (() => PortStateEnum.Open, false, false),
                (() => (PortStateEnum)1, false, false)
            };
            // act & assert
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfInvalidEnum(value.expression), "ArgumentNullException expected but not thrown.");
                }                                                    
                if (value.shouldRaceArgument)
                {
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfInvalidEnum(value.expression), "ArgumentException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceArgument)
                {
                    NAssert.DoesNotThrow((() => CheckUtil.ThrowIfInvalidEnum(value.expression)), "Unexpected exception.");
                }                
            }            
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="CheckUtil.ThrowIfNull{T}" />.
        /// </summary>
        [TestMethod]
        public void ThrowIfNullTest()
        {
            // arrange
            var values = new List<(Expression<Func<object>> expression, bool shouldRaceNull, bool shouldRaceRefNull)>
            {
                (null, true, false),
                (() => null, false, true),
                (() => "Hello", false, false),
                (() => DateTime.Now, false, false),
                (() => default(DateTime?), false, true),
                (() => default(int?), false, true)
            };
            // act & assert
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfNull(value.expression), "ArgumentNullException expected but not thrown.");
                }
                if (value.shouldRaceRefNull)
                {
                    NAssert.Throws<NullReferenceException>(() => CheckUtil.ThrowIfNull(value.expression), "NullReferenceException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceRefNull)
                {
                    NAssert.DoesNotThrow((() => CheckUtil.ThrowIfNull(value.expression)), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="CheckUtil.ThrowIfNullOrEmpty" />.
        /// </summary>
        [TestMethod]
        public void ThrowIfNullOrEmptyTest()
        {
            // arrange
            var values = new List<(Expression<Func<string>> expression, bool shouldRaceNull, bool shouldRaceArg)>
            {
                (null, true, false),
                (() => string.Empty, false, true),
                (() => null, false, true),
                (() => "Hello World!", false, false),
                (() => "1", false, false),
                (() => " ", false, false)
            };
            // act & assert
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfNullOrEmpty(value.expression), "ArgumentNullException expected but not thrown.");
                }
                if (value.shouldRaceArg)
                {
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfNullOrEmpty(value.expression), "ArgumentException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceArg)
                {
                    NAssert.DoesNotThrow((() => CheckUtil.ThrowIfNullOrEmpty(value.expression)), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="CheckUtil.ThrowIfNullOrWhitespace" />.
        /// </summary>
        [TestMethod]
        public void ThrowIfNullOrWhitespaceTest()
        {
            // arrange
            var values = new List<(Expression<Func<string>> expression, bool shouldRaceNull, bool shouldRaceArg)>
            {
                (null, true, false),
                (() => string.Empty, false, true),
                (() => null, false, true),
                (() => "Hello World!", false, false),
                (() => "1", false, false),
                (() => " ", false, true),
                (() => "  ", false, true),
                (() => new string(' ', 1000), false, true)
            };
            // act & assert
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfNullOrWhitespace(value.expression), "ArgumentNullException expected but not thrown.");
                }
                if (value.shouldRaceArg)
                {
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfNullOrWhitespace(value.expression), "ArgumentException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceArg)
                {
                    NAssert.DoesNotThrow((() => CheckUtil.ThrowIfNullOrWhitespace(value.expression)), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="CheckUtil.ThrowIfZeroOrNegative(Expression{System.Func{long}},Action{System.Exception})" />.
        /// </summary>
        [TestMethod]
        public void ThrowIfZeroOrNegativeLongTest()
        {
            // arrange
            var values = new List<(Expression<Func<long>> expression, bool shouldRaceNull, bool shouldRaceArg)>
            {
                (null, true, false)   ,
                (() => 0L, false, true),
                (() => 1L, false, false),
                (() => long.MinValue, false, true),
                (() => long.MaxValue, false, false),
                (() => (long)1.5, false, false),
                (() => (long).5, false, true),
                (() => (long).99, false, true)
            };
            // act & assert
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfZeroOrNegative(value.expression), "ArgumentNullException expected but not thrown.");
                }
                if (value.shouldRaceArg)
                {
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfZeroOrNegative(value.expression), "ArgumentException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceArg)
                {
                    NAssert.DoesNotThrow((() => CheckUtil.ThrowIfZeroOrNegative(value.expression)), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="CheckUtil.ThrowIfZeroOrNegative(Expression{System.Func{int}},Action{System.Exception})" />.
        /// </summary>
        [TestMethod]
        public void ThrowIfZeroOrNegativeIntTest()
        {
            // arrange
            var values = new List<(Expression<Func<int>> expression, bool shouldRaceNull, bool shouldRaceArg)>
            {
                (null, true, false)   ,
                (() => 0, false, true),
                (() => 1, false, false),
                (() => int.MinValue, false, true),
                (() => int.MaxValue, false, false),
                (() => (int)1.5, false, false),
                (() => (int).5, false, true),
                (() => (int).99, false, true)
            };
            // act & assert
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfZeroOrNegative(value.expression), "ArgumentNullException expected but not thrown.");
                }
                if (value.shouldRaceArg)
                {
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfZeroOrNegative(value.expression), "ArgumentException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceArg)
                {
                    NAssert.DoesNotThrow((() => CheckUtil.ThrowIfZeroOrNegative(value.expression)), "Unexpected exception.");
                }
            }
        }

        #endregion
    }
}