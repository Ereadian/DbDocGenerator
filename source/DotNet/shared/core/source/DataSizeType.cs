//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DataSizeType.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Data size type
    /// </summary>
    /// <remarks>
    /// Some types do not require size. For example, integer, data time, unique identifier, and so on.
    /// Some types like VARCHAR, NVARCHAR, requires size.
    /// Some types like VARCHAR, NVARCHAR, also support "MAX"
    /// </remarks>
    public enum DataSizeType
    {
        /// <summary>
        /// Data size is not required
        /// </summary>
        NotRequired,

        /// <summary>
        /// Data size is defined
        /// </summary>
        Required,

        /// <summary>
        /// Data size is "Maximum"
        /// </summary>
        Maximum
    }
}
