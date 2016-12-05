//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IDataTypeFactory.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Data type factory
    /// </summary>
    public interface IDataTypeFactory
    {
        /// <summary>
        /// Get data type instance
        /// </summary>
        /// <param name="name">data type name</param>
        /// <param name="size">data size</param>
        /// <returns>data type instance</returns>
        /// <remarks>
        /// if data type does not require size or it is max, the size parameter should be null and the method will return a re-useable 
        /// instance. If not, it will create a new instance.
        /// </remarks>
        IDataType GetDataType(string name, int? size = null);
    }
}
