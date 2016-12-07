//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IDatabaseConfiguration.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Database configuration
    /// </summary>
    public interface IDatabaseConfiguration
    {
        /// <summary>
        /// Gets database supported types
        /// </summary>
        IReadOnlyDictionary<string, IDataType> SupportedTypes { get; }

        /// <summary>
        /// Gets query commands
        /// </summary>
        IReadOnlyDictionary<string, string> QueryCommands { get; }
    }
}
