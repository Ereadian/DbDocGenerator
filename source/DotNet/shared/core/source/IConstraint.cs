//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IConstraint.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Table constraint
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// Gets or set the table the constraint belongs to
        /// </summary>
        ITable Table { get; set; }

        /// <summary>
        /// Gets or sets constraint display name
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets constraint schema name
        /// </summary>
        string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets constraint name
        /// </summary>
        string ConstraintName { get; set; }

        /// <summary>
        /// Gets or sets constraint columns
        /// </summary>
        IReadOnlyList<KeyValuePair<IColumn, IColumn>> Columns { get; set; }
    }
}
