//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DataType.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// IDataType implementation
    /// </summary>
    internal class DataType : IDataType
    {
        /// <summary>
        /// Data size
        /// </summary>
        private readonly IDataSize dataSize;

        /// <summary>
        /// Data type name
        /// </summary>
        private readonly string typeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataType" /> class.
        /// </summary>
        /// <param name="name">type name</param>
        /// <param name="size">type size</param>
        internal DataType(string name, IDataSize size)
        {
            this.dataSize = size;
            this.typeName = name;
        }

        /// <summary>
        /// Gets data size
        /// </summary>
        public IDataSize DataSize
        {
            get
            {
                return this.dataSize;
            }
        }

        /// <summary>
        /// Gets data type name
        /// </summary>
        public string TypeName
        {
            get
            {
                return this.typeName;
            }
        }
    }
}
