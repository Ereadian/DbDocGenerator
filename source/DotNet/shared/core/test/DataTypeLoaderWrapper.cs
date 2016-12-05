//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DataTypeLoaderWrapper.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Test
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;

    /// <summary>
    /// Data type loader wrapper
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class DataTypeLoaderWrapper : DataTypeLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeLoaderWrapper" /> class.
        /// </summary>
        /// <param name="configurationRootXml">configuration XML</param>
        public DataTypeLoaderWrapper(XmlElement configurationRootXml) : base(configurationRootXml)
        {
        }

        /// <summary>
        /// Get data type repository
        /// </summary>
        /// <returns>repository instance</returns>
        internal IReadOnlyDictionary<string, IReadOnlyDictionary<string, IDataType>> GetRepository()
        {
            return this.DataTypeRepository;
        }
    }
}
