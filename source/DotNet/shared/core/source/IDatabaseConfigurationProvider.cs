//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IDatabaseConfigurationProvider.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Database provider configuration loader
    /// </summary>
    public interface IDatabaseConfigurationProvider
    {
        /// <summary>
        /// Gets all database provider names
        /// </summary>
        IReadOnlyCollection<string> SupportedDatabaseProviderNames { get; }

        /// <summary>
        /// Gets database provider configuration by given provider name
        /// </summary>
        /// <param name="databaseProviderName">database provider name</param>
        /// <returns>database provider configuration</returns>
        IDatabaseConfiguration this[string databaseProviderName] { get; }
    }
}
