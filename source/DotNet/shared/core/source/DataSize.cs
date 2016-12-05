//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DataSize.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// IDataSize implementation
    /// </summary>
    internal class DataSize : IDataSize
    {
        /// <summary>
        /// Data size type
        /// </summary>
        private readonly DataSizeType dataSizeType;

        /// <summary>
        /// Data size
        /// </summary>
        private readonly int? dataSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSize" /> class.
        /// </summary>
        /// <param name="type">data type</param>
        /// <param name="size">data size</param>
        internal DataSize(DataSizeType type, int? size)
        {
            this.dataSizeType = type;
            this.dataSize = size;
        }

        /// <summary>
        /// Gets data size
        /// </summary>
        public int? Size
        {
            get
            {
                return this.dataSize;
            }
        }

        /// <summary>
        /// Gets data size type
        /// </summary>
        public DataSizeType SizeType
        {
            get
            {
                return this.dataSizeType;
            }
        }
    }
}
