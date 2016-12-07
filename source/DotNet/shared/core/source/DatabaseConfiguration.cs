//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DatabaseConfiguration.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Database configuration
    /// </summary>
    internal class DatabaseConfiguration : IDatabaseConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfiguration" /> class.
        /// </summary>
        /// <param name="supportedTypes">support types</param>
        /// <param name="queryCommands">query commands</param>
        internal DatabaseConfiguration(
            IReadOnlyDictionary<string, IDataType> supportedTypes,
            IReadOnlyDictionary<string, string> queryCommands)
        {
            this.SupportedTypes = supportedTypes;
            this.QueryCommands = queryCommands;
        }

        /// <summary>
        /// Gets database supported types
        /// </summary>
        public IReadOnlyDictionary<string, IDataType> SupportedTypes { get; private set; }

        /// <summary>
        /// Gets query commands
        /// </summary>
        public IReadOnlyDictionary<string, string> QueryCommands { get; private set; }
    }
}
