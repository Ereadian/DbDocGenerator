//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IParameter.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Routine parameter interface
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Gets or sets the the routine the parameter belongs to
        /// </summary>
        IRoutine Routine { get; set; }

        /// <summary>
        /// Gets or set the parameter name
        /// </summary>
        string ParameterName { get; set; }

        /// <summary>
        /// Gets or set the parameter mode/direction
        /// </summary>
        string Mode { get; set; }

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
        /// Gets default column value
        /// </summary>
        string DefaultValue { get; set; }
    }
}
