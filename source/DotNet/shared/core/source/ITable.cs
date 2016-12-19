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
        IList<IColumn> Columns { get; set; }
    }
}
