//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IIndex.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Index interface
    /// </summary>
    public interface IIndex
    {
        /// <summary>
        /// Gets or sets the table instance which the index belongs to
        /// </summary>
        ITable Table { get; set; }

        /// <summary>
        /// Gets or sets the index display like [DBO].[IX_MyTableId]
        /// </summary>
        string IndexDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the schema name of the index
        /// </summary>
        string IndexSchema { get; set; }

        /// <summary>
        /// Gets or sets the name of the index
        /// </summary>
        string IndexName { get; set; }

        /// <summary>
        /// Gets or sets the associated columns
        /// </summary>
        IReadOnlyList<IColumn> Columns { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the index is primary key
        /// </summary>
        bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the index is unique index
        /// </summary>
        bool IsUniqueIndex { get; set; }
    }
}
