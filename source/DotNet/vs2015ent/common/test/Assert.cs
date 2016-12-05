//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="Assert.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Common.Test
{
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
    }
}
