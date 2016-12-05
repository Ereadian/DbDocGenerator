//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IDataType.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Data type description
    /// </summary>
    public interface IDataType
    {
        /// <summary>
        /// Gets or sets data type name
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Gets flag whether data requires size
        /// </summary>
        bool RequireSize { get; }
    }
}
