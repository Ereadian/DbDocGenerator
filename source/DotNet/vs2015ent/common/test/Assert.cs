//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="Assert.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Common.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Original = Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// MS Unit test Assert wrapper
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Assert
    {
        /// <summary>
        /// Verifies that two specified generic type data are equal by using the equality
        /// operator. The assertion fails if they are not equal.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="expected">
        /// The first generic type data to compare. This is the generic type data the unit test expects.
        /// </param>
        /// <param name="actual">
        /// The second generic type data to compare. This is the generic type data the unit test produced.
        /// </param>
        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">
        /// expected is not equal to actual
        /// </exception>
        public static void AreEqual<T>(T expected, T actual)
        {
            Original.Assert.AreEqual<T>(expected, actual);
        }

        /// <summary>
        /// Check two dictionaries. Throw exception if they are not same
        /// </summary>
        /// <typeparam name="TKey">type of key</typeparam>
        /// <typeparam name="TValue">type of value</typeparam>
        /// <param name="expected">expected dictionary</param>
        /// <param name="actual">actual dictionary</param>
        /// <param name="compareValues">callback for verify dictionary value</param>
        public static void AreDictionaryEqual<TKey, TValue>(
            IDictionary<TKey, TValue> expected,
            IDictionary<TKey, TValue> actual,
            Action<TValue, TValue> compareValues = null)
        {
            TestUtility.AreDictionaryEqual(expected, actual, compareValues);
        }

        /// <summary>
        /// Check two dictionaries. Throw exception if they are not same
        /// </summary>
        /// <typeparam name="TKey">type of key</typeparam>
        /// <typeparam name="TValue">type of value</typeparam>
        /// <param name="expected">expected dictionary</param>
        /// <param name="actual">actual dictionary</param>
        /// <param name="compareValues">callback for verify dictionary value</param>
        public static void AreReadOnlyDictionaryEqual<TKey, TValue>(
            IReadOnlyDictionary<TKey, TValue> expected,
            IReadOnlyDictionary<TKey, TValue> actual,
            Action<TValue, TValue> compareValues = null)
        {
            TestUtility.AreReadOnlyDictionaryEqual(expected, actual, compareValues);
        }

        /// <summary>
        /// Verifies that the specified object is null. The assertion fails if it is not null.
        /// </summary>
        /// <param name="value">The object to verify is null</param>
        /// <exception cref="Original.AssertFailedException">
        /// value is not null
        /// </exception>
        public static void IsNull(object value)
        {
            Original.Assert.IsNull(value);
        }

        /// <summary>
        /// Verifies that the specified object is not null. The assertion fails if it is null.
        /// </summary>
        /// <param name="value">The object to verify is not null.</param>
        /// <exception cref="Original.AssertFailedException">value is null</exception>
        public static void IsNotNull(object value)
        {
            Original.Assert.IsNotNull(value);
        }

        /// <summary>
        /// Verifies that the specified condition is true. The assertion fails if the condition is false.
        /// </summary>
        /// <param name="condition">he condition to verify is true</param>
        /// <exception cref="Original.AssertFailedException">condition evaluates to false</exception>
        public static void IsTrue(bool condition)
        {
            Original.Assert.IsTrue(condition);
        }
    }
}
