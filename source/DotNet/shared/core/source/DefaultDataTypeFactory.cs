//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DefaultDataTypeFactory.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Default data type factory
    /// </summary>
    public class DefaultDataTypeFactory : IDataTypeFactory
    {
        /// <summary>
        /// data type supported by current provider
        /// </summary>
        private readonly IReadOnlyDictionary<string, IDataType> dataTypeCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataTypeFactory" /> class.
        /// </summary>
        public DefaultDataTypeFactory() : this(DatabaseProviderName.MsSql)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataTypeFactory" /> class.
        /// </summary>
        /// <param name="providerName">provider name</param>
        public DefaultDataTypeFactory(string providerName) : this((new DataTypeLoader())[providerName])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataTypeFactory" /> class.
        /// </summary>
        /// <param name="typeCollection">data types which supported by current provider</param>
        public DefaultDataTypeFactory(IReadOnlyDictionary<string, IDataType> typeCollection)
        {
            if (typeCollection == null)
            {
                throw new ArgumentNullException("typeCollection");
            }

            this.dataTypeCollection = typeCollection;
        }

        /// <summary>
        /// Get data type by type name
        /// </summary>
        /// <param name="name">data type name</param>
        /// <param name="size">size of data</param>
        /// <returns>data type instance</returns>
        public IDataType GetDataType(string name, int? size = default(int?))
        {
            if (string.IsNullOrEmpty(name))
            {
                const string ErrorMessage = "parameter \"name\" should not be null or blank";
                Trace.TraceError(ErrorMessage);
                throw new ArgumentException(ErrorMessage);
            }

            IDataType dataType;
            if (!this.dataTypeCollection.TryGetValue(name, out dataType))
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Type \"{0}\" is not supported",
                    name);
                Trace.TraceError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            var sizeType = dataType.DataSize.SizeType;
            if ((sizeType == DataSizeType.NotRequired) || ((sizeType == DataSizeType.Maximum) && !size.HasValue))
            {
                return dataType;
            }

            if (!size.HasValue)
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Data size of type \"{0}\" is required",
                    name);
                Trace.TraceError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            return new DataType(dataType.TypeName, new DataSize(DataSizeType.Required, size));
        }
    }
}
