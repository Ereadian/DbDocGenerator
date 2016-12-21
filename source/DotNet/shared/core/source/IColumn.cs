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
        /// Gets or sets name of column data type
        /// </summary>
        string DataTypeName { get; set; }

        /// <summary>
        /// Gets or sets character size
        /// </summary>
        int? StringSize { get; set; }

        /// <summary>
        /// Gets or sets the size of column numeric precision
        /// </summary>
        int? NumericPrecision { get; set; }

        /// <summary>
        /// Gets or sets the size of column numeric scale
        /// </summary>
        int? NumericScale { get; set; }

        /// <summary>
        /// Gets the flag that allow column value be null
        /// </summary>
        bool IsNullable { get; set; }

        /// <summary>
        /// Gets default column value
        /// </summary>
        string DefaultValue { get; set; }
    }
}
