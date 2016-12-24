//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IReference.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Reference interface
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// Gets or sets referenced by entity display name
        /// </summary>
        string EntityDisplayName { get; set; }

        /// <summary>
        /// Gets or sets referenced by entity schema name
        /// </summary>
        string EntitySchema { get; set; }

        /// <summary>
        /// Gets or sets referenced by entity name
        /// </summary>
        string EntityName { get; set; }

        /// <summary>
        /// Gets or sets referenced by entity type
        /// </summary>
        string EntityType { get; set; }
    }
}
