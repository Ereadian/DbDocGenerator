//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="EnumeratorInformation.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Common.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Enumerator information for test
    /// </summary>
    /// <typeparam name="T">enumerator type</typeparam>
    [ExcludeFromCodeCoverage]
    public static class EnumeratorInformation<T>
    {
        /// <summary>
        /// Initializes static members of the <see cref="EnumeratorInformation{T}" /> class.
        /// </summary>
        static EnumeratorInformation()
        {
            var type = typeof(T);
            Names = Enum.GetNames(type);
            Values = Names.Select(name => (T)Enum.Parse(type, name)).ToArray();
        }

        /// <summary>
        /// Gets enumerator names
        /// </summary>
        public static string[] Names { get; }

        /// <summary>
        /// Gets enumerator values
        /// </summary>
        public static T[] Values { get; }
    }
}
