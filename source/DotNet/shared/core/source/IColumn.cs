//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IColumn.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Table column definition
    /// </summary>
    public interface IColumn
    {
        /// <summary>
        /// Gets or sets table which the column belongs to
        /// </summary>
        ITable Table { get; set; }

        /// <summary>
        /// Gets or sets column name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets type of the column
        /// </summary>
        IDataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the size of column data
        /// </summary>
        int? DataSize { get; set; }
    }
}
