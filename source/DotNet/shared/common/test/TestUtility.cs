//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="TestUtility.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Common.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// Test utilities
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class TestUtility
    {
        /// <summary>
        /// Create random name
        /// </summary>
        /// <param name="template">random template</param>
        /// <returns>random name based on template</returns>
        public static string CreateRandomName(string template)
        {
            return string.Format(CultureInfo.InvariantCulture, template, Guid.NewGuid().ToString("N"));
        }

        #region Assert
        /// <summary>
        /// Check two dictionaries. Throw exception if they are not same
        /// </summary>
        /// <typeparam name="TKey">type of key</typeparam>
        /// <typeparam name="TValue">type of value</typeparam>
        /// <param name="expected">expected dictionary</param>
        /// <param name="actual">actual dictionary</param>
        /// <param name="compareValues">callback for verify dictionary value</param>
        internal static void AreDictionaryEqual<TKey, TValue>(
            IDictionary<TKey, TValue> expected,
            IDictionary<TKey, TValue> actual,
            Action<TValue, TValue> compareValues = null)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.IsNotNull(actual);
            Assert.AreEqual<int>(expected.Count, actual.Count);
            foreach (var pair in expected)
            {
                TValue value;
                Assert.IsTrue(actual.TryGetValue(pair.Key, out value));
                if (compareValues != null)
                {
                    compareValues(pair.Value, value);
                }
                else
                {
                    Assert.AreEqual<TValue>(pair.Value, value);
                }
            }
        }

        /// <summary>
        /// Check two dictionaries. Throw exception if they are not same
        /// </summary>
        /// <typeparam name="TKey">type of key</typeparam>
        /// <typeparam name="TValue">type of value</typeparam>
        /// <param name="expected">expected dictionary</param>
        /// <param name="actual">actual dictionary</param>
        /// <param name="compareValues">callback for verify dictionary value</param>
        internal static void AreReadOnlyDictionaryEqual<TKey, TValue>(
            IReadOnlyDictionary<TKey, TValue> expected,
            IReadOnlyDictionary<TKey, TValue> actual,
            Action<TValue, TValue> compareValues = null)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.IsNotNull(actual);
            Assert.AreEqual<int>(expected.Count, actual.Count);
            foreach (var pair in expected)
            {
                TValue value;
                Assert.IsTrue(actual.TryGetValue(pair.Key, out value));
                if (compareValues != null)
                {
                    compareValues(pair.Value, value);
                }
                else
                {
                    Assert.AreEqual<TValue>(pair.Value, value);
                }
            }
        }
        #endregion Assert
    }
}
