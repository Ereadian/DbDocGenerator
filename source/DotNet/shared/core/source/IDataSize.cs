//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IDataSize.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Data size description
    /// </summary>
    public interface IDataSize
    {
        /// <summary>
        /// Gets data size type
        /// </summary>
        DataSizeType SizeType { get;  }

        /// <summary>
        /// Gets data size
        /// </summary>
        int? Size { get; }
    }
}
