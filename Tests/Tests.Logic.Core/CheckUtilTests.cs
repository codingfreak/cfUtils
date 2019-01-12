namespace codingfreaks.cfUtils.Tests.Logic.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using cfUtils.Logic.Core.Enumerations;
    using cfUtils.Logic.Core.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NAssert = NUnit.Framework.Assert;

    /// <summary>
    /// Contains unit tests for <see cref="CheckUtil" />.
    /// </summary>
    [TestClass]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CheckUtilTests
    {
        #region methods

        /// <summary>
        /// Checks if the method
        /// <see cref="CheckUtil.ThrowIfInvalidEnum{TEnum}" /> calls the optional callback correctly.
        /// </summary>
        [TestMethod]
        public void ThrowIfInvalidEnum_Callback_Test()
        {
            // arrange
            var values = new List<(Expression<Func<PortState>> expression, bool shouldRaceNull, bool shouldRaceArgument)>
            {
                (null, true, false),
                (() => (PortState)10, false, true),
                (() => PortState.Open, false, false),
                (() => (PortState)1, false, false)
            };
            var expectedCallbacks = values.Count(v => !v.shouldRaceNull && v.shouldRaceArgument);
            var actualCallbacks = 0;
            // act && assert for exceptions            
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    // should not race the callback
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfInvalidEnum(value.expression, ex => actualCallbacks++), "ArgumentNullException expected but not thrown.");
                }
                if (value.shouldRaceArgument)
                {
                    // should race the callback
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfInvalidEnum(value.expression, ex => actualCallbacks++), "ArgumentException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceArgument)
                {
                    // should not race the callback
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfInvalidEnum(value.expression, ex => actualCallbacks++), "Unexpected exception.");
                }
            }
            // assert for correct amount of callbacks called
            NAssert.AreEqual(expectedCallbacks, actualCallbacks, "Incorrect amount of callbacks was called.");
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="CheckUtil.ThrowIfInvalidEnum{TEnum}" /> when a discrete value is
        /// passed.
        /// </summary>
        [TestMethod]
        public void ThrowIfInvalidEnum_Test()
        {
            // arrange
            var values = new List<(Expression<Func<PortState>> expression, bool shouldRaceNull, bool shouldRaceArgument)>
            {
                (null, true, false),
                (() => (PortState)10, false, true),
                (() => PortState.Open, false, false),
                (() => (PortState)1, false, false)
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
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfInvalidEnum(value.expression), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Checks if the method <see cref="CheckUtil.ThrowIfNull{T}" /> calls the optional callback correctly.
        /// </summary>
        [TestMethod]
        public void ThrowIfNull_Callback_Test()
        {
            // arrange
            var values = new List<(SampleModel<string> model, bool shouldRaceRefNull)>
            {
                (new SampleModel<string>(null), true),
                (new SampleModel<string>(default), true),
                (new SampleModel<string>("Hello"), false),
                (new SampleModel<string>(string.Empty), false),
                (new SampleModel<string>(""), false),
                (new SampleModel<string>(" "), false)
            };
            var expectedCallbacks = values.Count(v => v.shouldRaceRefNull);
            var actualCallbacks = 0;
            // act && assert for exceptions            
            foreach (var value in values)
            {
                if (value.shouldRaceRefNull)
                {
                    NAssert.Throws<NullReferenceException>(() => CheckUtil.ThrowIfNull(() => value.model.SampleValue, ex => actualCallbacks++), "NullReferenceException expected but not thrown.");
                }
                else
                {
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfNull(() => value.model.SampleValue, ex => actualCallbacks++), "Unexpected exception.");
                }
            }
            // assert for correct amount of callbacks called
            NAssert.AreEqual(expectedCallbacks, actualCallbacks, "Incorrect amount of callbacks was called.");
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="CheckUtil.ThrowIfNull{T}" /> when a complex data type
        /// property is passed.
        /// </summary>
        [TestMethod]
        public void ThrowIfNull_Test()
        {
            // arrange
            var values = new List<(SampleModel<string> model, bool shouldRaceRefNull)>
            {
                (new SampleModel<string>(null), true),
                (new SampleModel<string>(default), true),
                (new SampleModel<string>("Hello"), false),
                (new SampleModel<string>(string.Empty), false),
                (new SampleModel<string>(""), false),
                (new SampleModel<string>(" "), false)
            };
            // act & assert
            NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfNull(default(Expression<Func<string>>)), "ArgumentNullException expected but not thrown");
            foreach (var value in values)
            {
                if (value.shouldRaceRefNull)
                {
                    NAssert.Throws<NullReferenceException>(() => CheckUtil.ThrowIfNull(() => value.model.SampleValue), "NullReferenceException expected but not thrown.");
                }
                else
                {
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfNull(() => value.model.SampleValue), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Checks if the method <see cref="CheckUtil.ThrowIfNullOrEmpty" /> calls the optional callback correctly.
        /// </summary>
        [TestMethod]
        public void ThrowIfNullOrEmpty_Callback_Test()
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
            var expectedCallbacks = values.Count(v => !v.shouldRaceNull && v.shouldRaceArg);
            var actualCallbacks = 0;
            // act && assert for exceptions            
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    // should not race the callback
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfNullOrEmpty(value.expression, ex => actualCallbacks++), "ArgumentNullException expected but not thrown.");
                }
                if (value.shouldRaceArg)
                {
                    // should race the callback
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfNullOrEmpty(value.expression, ex => actualCallbacks++), "ArgumentException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceArg)
                {
                    // should not race the callback
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfNullOrEmpty(value.expression, ex => actualCallbacks++), "Unexpected exception.");
                }
            }
            // assert for correct amount of callbacks called
            NAssert.AreEqual(expectedCallbacks, actualCallbacks, "Incorrect amount of callbacks was called.");
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="CheckUtil.ThrowIfNullOrEmpty" /> when a discrete value is
        /// passed.
        /// </summary>
        [TestMethod]
        public void ThrowIfNullOrEmpty_Test()
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
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfNullOrEmpty(value.expression), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Checks if the method <see cref="CheckUtil.ThrowIfNullOrWhitespace" /> calls the optional callback correctly.
        /// </summary>
        [TestMethod]
        public void ThrowIfNullOrWhitespace_Callback_Test()
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
            var expectedCallbacks = values.Count(v => !v.shouldRaceNull && v.shouldRaceArg);
            var actualCallbacks = 0;
            // act && assert for exceptions            
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    // should not race the callback
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfNullOrWhitespace(value.expression, ex => actualCallbacks++), "ArgumentNullException expected but not thrown.");
                }
                if (value.shouldRaceArg)
                {
                    // should race the callback
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfNullOrWhitespace(value.expression, ex => actualCallbacks++), "ArgumentException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceArg)
                {
                    // should not race the callback
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfNullOrWhitespace(value.expression, ex => actualCallbacks++), "Unexpected exception.");
                }
            }
            // assert for correct amount of callbacks called
            NAssert.AreEqual(expectedCallbacks, actualCallbacks, "Incorrect amount of callbacks was called.");
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="CheckUtil.ThrowIfNullOrWhitespace" /> when a discrete value is
        /// passed.
        /// </summary>
        [TestMethod]
        public void ThrowIfNullOrWhitespace_Test()
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
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfNullOrWhitespace(value.expression), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Tests the correct functionality of
        /// <see cref="CheckUtil.ThrowIfZeroOrNegative(System.Linq.Expressions.Expression{System.Func{int}},Action{Exception})" />
        /// when a complex data type
        /// property is passed.
        /// </summary>
        [TestMethod]
        public void ThrowIfNullTestMember_Int_Test()
        {
            // arrange
            var values = new List<(SampleModel<int> model, bool shouldRaceArg)>
            {
                (new SampleModel<int>(), true),
                (new SampleModel<int>(0), true),
                (new SampleModel<int>(int.MinValue), true),
                (new SampleModel<int>(int.MaxValue), false),
                (new SampleModel<int>((int)1.5), false),
                (new SampleModel<int>((int).5), true),
                (new SampleModel<int>((int).99), true)
            };
            // act & assert
            foreach (var value in values)
            {
                if (value.shouldRaceArg)
                {
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfZeroOrNegative(() => value.model.SampleValue), "ArgumentException expected but not thrown.");
                }
                else
                {
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfZeroOrNegative(() => value.model.SampleValue), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Checks if the method
        /// <see cref="CheckUtil.ThrowIfZeroOrNegative(System.Linq.Expressions.Expression{System.Func{int}},Action{System.Exception})" />
        /// calls the optional callback correctly.
        /// </summary>
        [TestMethod]
        public void ThrowIfZeroOrNegative_Int_Callback_Test()
        {
            // arrange
            var values = new List<(Expression<Func<int>> expression, bool shouldRaceNull, bool shouldRaceArg)>
            {
                (null, true, false),
                (() => 0, false, true),
                (() => 1, false, false),
                (() => int.MinValue, false, true),
                (() => int.MaxValue, false, false),
                (() => (int)1.5, false, false),
                (() => (int).5, false, true),
                (() => (int).99, false, true)
            };
            var expectedCallbacks = values.Count(v => !v.shouldRaceNull && v.shouldRaceArg);
            var actualCallbacks = 0;
            // act && assert for exceptions            
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    // should not race the callback
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfZeroOrNegative(value.expression, ex => actualCallbacks++), "ArgumentNullException expected but not thrown.");
                }
                if (value.shouldRaceArg)
                {
                    // should race the callback
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfZeroOrNegative(value.expression, ex => actualCallbacks++), "ArgumentException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceArg)
                {
                    // should not race the callback
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfZeroOrNegative(value.expression, ex => actualCallbacks++), "Unexpected exception.");
                }
            }
            // assert for correct amount of callbacks called
            NAssert.AreEqual(expectedCallbacks, actualCallbacks, "Incorrect amount of callbacks was called.");
        }

        /// <summary>
        /// Tests the correct functionality of
        /// <see cref="CheckUtil.ThrowIfZeroOrNegative(System.Linq.Expressions.Expression{System.Func{long}},Action{Exception})" />
        /// .
        /// </summary>
        [TestMethod]
        public void ThrowIfZeroOrNegative_Int_Test()
        {
            // arrange
            var values = new List<(Expression<Func<int>> expression, bool shouldRaceNull, bool shouldRaceArg)>
            {
                (null, true, false),
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
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfZeroOrNegative(value.expression), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Checks if the method
        /// <see cref="CheckUtil.ThrowIfZeroOrNegative(System.Linq.Expressions.Expression{System.Func{int}},Action{Exception})" />
        /// calls the
        /// optional callback correctly.
        /// </summary>
        [TestMethod]
        public void ThrowIfZeroOrNegative_Long_Callback_Test()
        {
            // arrange
            var values = new List<(Expression<Func<long>> expression, bool shouldRaceNull, bool shouldRaceArg)>
            {
                (null, true, false),
                (() => 0L, false, true),
                (() => 1L, false, false),
                (() => long.MinValue, false, true),
                (() => long.MaxValue, false, false),
                (() => (long)1.5, false, false),
                (() => (long).5, false, true),
                (() => (long).99, false, true)
            };
            var expectedCallbacks = values.Count(v => !v.shouldRaceNull && v.shouldRaceArg);
            var actualCallbacks = 0;
            // act && assert for exceptions            
            foreach (var value in values)
            {
                if (value.shouldRaceNull)
                {
                    // should not race the callback
                    NAssert.Throws<ArgumentNullException>(() => CheckUtil.ThrowIfZeroOrNegative(value.expression, ex => actualCallbacks++), "ArgumentNullException expected but not thrown.");
                }
                if (value.shouldRaceArg)
                {
                    // should race the callback
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfZeroOrNegative(value.expression, ex => actualCallbacks++), "ArgumentException expected but not thrown.");
                }
                if (!value.shouldRaceNull && !value.shouldRaceArg)
                {
                    // should not race the callback
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfZeroOrNegative(value.expression, ex => actualCallbacks++), "Unexpected exception.");
                }
            }
            // assert for correct amount of callbacks called
            NAssert.AreEqual(expectedCallbacks, actualCallbacks, "Incorrect amount of callbacks was called.");
        }

        /// <summary>
        /// Tests the correct functionality of
        /// <see cref="CheckUtil.ThrowIfZeroOrNegative(System.Linq.Expressions.Expression{System.Func{long}},Action{Exception})" />
        /// when a discrete value is
        /// passed.
        /// </summary>
        [TestMethod]
        public void ThrowIfZeroOrNegative_Long_Test()
        {
            // arrange
            var values = new List<(Expression<Func<long>> expression, bool shouldRaceNull, bool shouldRaceArg)>
            {
                (null, true, false),
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
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfZeroOrNegative(value.expression), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Tests the correct functionality of
        /// <see cref="CheckUtil.ThrowIfZeroOrNegative(System.Linq.Expressions.Expression{System.Func{int}},Action{Exception})" />
        /// when a complex data type
        /// property is passed.
        /// </summary>
        [TestMethod]
        public void ThrowIfZeroOrNegative_Member_Int_Test()
        {
            // arrange
            var values = new List<(SampleModel<int> model, bool shouldRaceArg)>
            {
                (new SampleModel<int>(), true),
                (new SampleModel<int>(0), true),
                (new SampleModel<int>(int.MinValue), true),
                (new SampleModel<int>(int.MaxValue), false),
                (new SampleModel<int>((int)1.5), false),
                (new SampleModel<int>((int).5), true),
                (new SampleModel<int>((int).99), true)
            };
            // act & assert
            foreach (var value in values)
            {
                if (value.shouldRaceArg)
                {
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfZeroOrNegative(() => value.model.SampleValue), "ArgumentException expected but not thrown.");
                }
                else
                {
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfZeroOrNegative(() => value.model.SampleValue), "Unexpected exception.");
                }
            }
        }

        /// <summary>
        /// Tests the correct functionality of
        /// <see cref="CheckUtil.ThrowIfZeroOrNegative(Expression{Func{long}},Action{Exception})" /> when a complex data type
        /// property is passed.
        /// </summary>
        [TestMethod]
        public void ThrowIfZeroOrNegative_Member_Long_Test()
        {
            // arrange
            var values = new List<(SampleModel<long> model, bool shouldRaceArg)>
            {
                (new SampleModel<long>(), true),
                (new SampleModel<long>(0), true),
                (new SampleModel<long>(long.MinValue), true),
                (new SampleModel<long>(long.MaxValue), false),
                (new SampleModel<long>((long)1.5), false),
                (new SampleModel<long>((long).5), true),
                (new SampleModel<long>((long).99), true)
            };
            // act & assert
            foreach (var value in values)
            {
                if (value.shouldRaceArg)
                {
                    NAssert.Throws<ArgumentException>(() => CheckUtil.ThrowIfZeroOrNegative(() => value.model.SampleValue), "ArgumentException expected but not thrown.");
                }
                else
                {
                    NAssert.DoesNotThrow(() => CheckUtil.ThrowIfZeroOrNegative(() => value.model.SampleValue), "Unexpected exception.");
                }
            }
        }

        #endregion

        /// <summary>
        /// Serves as an internal data structure when complex models should be passed.
        /// </summary>
        internal struct SampleModel<T>
        {
            #region constructors and destructors

            public SampleModel(T sampleValue)
            {
                SampleValue = sampleValue;
            }

            #endregion

            #region properties

            /// <summary>
            /// Sets or gets the discrete value.
            /// </summary>
            public T SampleValue { get; }

            #endregion
        }
    }
}