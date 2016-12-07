//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="Singleton.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Singleton implementation
    /// </summary>
    /// <typeparam name="T">type of singleton instance</typeparam>
    public class Singleton<T> where T : new()
    {
        /// <summary>
        /// Gets singleton instance
        /// </summary>
        public static T Instance
        {
            get
            {
                return Storage.Instance;
            }
        }

        /// <summary>
        /// Singleton instance storage/initializer
        /// </summary>
        private static class Storage
        {
            /// <summary>
            /// Initializes static members of the <see cref="Storage" /> class.
            /// </summary>
            static Storage()
            {
                try
                {
                    Instance = new T();
                }
                catch (Exception exception)
                {
                    var errorMessage = string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to create instance of type {0}. Exception:{1}",
                        typeof(T).FullName,
                        exception.ToString());
                    Trace.TraceError(errorMessage);
                }
            }

            /// <summary>
            /// Gets singleton instance
            /// </summary>
            internal static T Instance { get; private set; }
        }
    }
}
