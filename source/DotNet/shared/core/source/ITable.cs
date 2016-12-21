//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="ITable.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Table definition interface
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// Table display name
        /// </summary>
        /// <example>[dbo].[Users]</example>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets Schema Name (for example, "DBO")
        /// </summary>
        string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets Table Name
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// Gets or sets columns
        /// </summary>
        IReadOnlyList<IColumn> Columns { get; set; }

        /// <summary>
        /// Gets or sets primary key
        /// </summary>
        IConstraint PrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets foreign keys
        /// </summary>
        IReadOnlyList<IConstraint> ForeignKeys { get; set; }
    }
}
