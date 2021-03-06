﻿//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IRoutine.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Routine interface
    /// </summary>
    public interface IRoutine
    {
        /// <summary>
        /// Routine display name
        /// </summary>
        /// <example>[dbo].[spGetUser]</example>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets Schema Name (for example, "DBO")
        /// </summary>
        string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets Routine Name
        /// </summary>
        string RoutineName { get; set; }

        /// <summary>
        /// Gets or sets routine type
        /// </summary>
        /// <remarks>
        /// It can be "PROCEDURE", "FUNCTION" and so on.
        /// </remarks>
        string RoutineType { get; set; }

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
        /// Gets or sets source code
        /// </summary>
        string SoruceCode { get; set; }

        /// <summary>
        /// Gets or sets parameters
        /// </summary>
        IReadOnlyList<IParameter> Parameters { get; set; }

        /// <summary>
        /// referenced by entities
        /// </summary>
        /// <remarks>
        /// key is [type][schema][name]. for example, fn.dbo.fnGetUser.
        /// </remarks>
        SortedList<string, IReference> References { get; set; }
    }
}
