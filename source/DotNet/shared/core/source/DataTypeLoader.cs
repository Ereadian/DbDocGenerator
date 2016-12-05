//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DataTypeLoader.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml;

    /// <summary>
    /// Load data types
    /// </summary>
    public class DataTypeLoader
    {
        #region XML names
        /// <summary>
        /// Provider XML element name
        /// </summary>
        public const string ProviderXmlElementName = "provider";

        /// <summary>
        /// Provider name attribute
        /// </summary>
        public const string ProviderNameAttributeName = "name";

        /// <summary>
        /// Type XML element name
        /// </summary>
        public const string TypeXmlElementName = "type";

        /// <summary>
        /// Type name attribute
        /// </summary>
        public const string TypeNameAttributeName = "name";

        /// <summary>
        /// Type size attribute
        /// </summary>
        public const string SizeNameAttributeName = "size";
        #endregion XML names

        #region Size attribute values
        /// <summary>
        /// size is not required
        /// </summary>
        public const string DataSizeTypeConfigurationNameForNotRequired = "no";

        /// <summary>
        /// Size is required
        /// </summary>
        public const string DataSizeTypeConfigurationNameForRequired = "required";

        /// <summary>
        /// Size is required and can apply "MAX"
        /// </summary>
        public const string DataSizeTypeConfigurationNameForAllowMax = "maximum";
        #endregion Size attribute values

        /// <summary>
        /// Configuration XML filename
        /// </summary>
        private const string TypesConfigurationXmlFilename = "types.xml";

        /// <summary>
        /// Gets data size name-type mapping 
        /// </summary>
        private static readonly IReadOnlyDictionary<string, DataSizeType> DataSizeTypeConfigurationNameValueMapping
            = new Dictionary<string, DataSizeType>(StringComparer.OrdinalIgnoreCase)
            {
                { DataSizeTypeConfigurationNameForNotRequired, DataSizeType.NotRequired },
                { DataSizeTypeConfigurationNameForRequired, DataSizeType.Required },
                { DataSizeTypeConfigurationNameForAllowMax, DataSizeType.Maximum },
            };

        /// <summary>
        /// types per provider repository
        /// </summary>
        private IReadOnlyDictionary<string, IReadOnlyDictionary<string, IDataType>> dataTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeLoader" /> class.
        /// </summary>
        public DataTypeLoader() : this(LoadConfigurationXml(TypesConfigurationXmlFilename))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeLoader" /> class.
        /// </summary>
        /// <param name="configurationRootXml">configuration XML</param>
        public DataTypeLoader(XmlElement configurationRootXml)
        {
            var repository = new Dictionary<string, IReadOnlyDictionary<string, IDataType>>(StringComparer.OrdinalIgnoreCase);
            this.dataTypeRepository = repository;
            if (configurationRootXml != null)
            {
                LoadProviders(repository, configurationRootXml, null);
            }
        }

        /// <summary>
        /// Gets type collection by provider name
        /// </summary>
        /// <param name="name">provider name</param>
        /// <returns>types supported by this provide</returns>
        public IReadOnlyDictionary<string, IDataType> this[string name]
        {
            get
            {
                IReadOnlyDictionary<string, IDataType> collection;
                return this.dataTypeRepository.TryGetValue(name, out collection) ? collection : null;
            }
        }

        /// <summary>
        /// Load configuration XML from embedded resources
        /// </summary>
        /// <param name="filename">types configuration file name</param>
        /// <returns>configuration XML. If not found, null will be returned</returns>
        private static XmlElement LoadConfigurationXml(string filename)
        {
            XmlElement configurationXml = null;
            var type = typeof(DataTypeLoader);
            using (var stream = type.Assembly.GetManifestResourceStream(type, filename))
            {
                if (stream == null)
                {
                    var errorMessage = string.Format(
                        CultureInfo.InvariantCulture,
                        "Embedded file \"{0}\" was not find in assembly \"{1}\".",
                        filename,
                        type.Assembly.FullName);
                    Trace.TraceError(errorMessage);
                    throw new ArgumentException(errorMessage);
                }

                var document = new XmlDocument();
                document.Load(stream);

                configurationXml = document.DocumentElement;
            }

            return configurationXml;
        }

        /// <summary>
        /// Load providers
        /// </summary>
        /// <param name="repository">provider repository</param>
        /// <param name="providerXml">provider XML</param>
        /// <param name="parentTypes">parent type collection</param>
        private static void LoadProviders(
            IDictionary<string, IReadOnlyDictionary<string, IDataType>> repository,
            XmlElement providerXml,
            IReadOnlyDictionary<string, IDataType> parentTypes)
        {
            var types = LoadTypes(providerXml, parentTypes);
            if (types != null)
            {
                string providerName = providerXml.GetAttribute(ProviderNameAttributeName);
                if (!string.IsNullOrWhiteSpace(providerName))
                {
                    providerName = providerName.Trim();
                    if (repository.ContainsKey(providerName))
                    {
                        var errorMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Provider name \"{0}\" has been defined. Duplicate names were found. XML: {1}",
                            providerName,
                            providerXml.OuterXml);
                        Trace.TraceError(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    repository.Add(providerName, types);
                }
            }

            var providerNodeCollection = providerXml.SelectNodes(ProviderXmlElementName);
            if (providerNodeCollection != null)
            {
                foreach (XmlNode providerNode in providerNodeCollection)
                {
                    var inheritedProviderXml = providerNode as XmlElement;
                    if (inheritedProviderXml == null)
                    {
                        var errorMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Expect \"{0}\" element but it is not. XML: {1}",
                            ProviderXmlElementName,
                            providerNode.OuterXml);
                        Trace.TraceError(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    LoadProviders(repository, inheritedProviderXml, types);
                }
            }
        }

        /// <summary>
        /// Load types of current provider
        /// </summary>
        /// <param name="providerXml">provider XML</param>
        /// <param name="parentTypes">types supported by parent provider</param>
        /// <returns>types supported by current provider</returns>
        private static IReadOnlyDictionary<string, IDataType> LoadTypes(
            XmlElement providerXml, 
            IReadOnlyDictionary<string, IDataType> parentTypes)
        {
            Dictionary<string, IDataType> typesSupportedByProvider = null;
            var typeNodeCollection = providerXml.SelectNodes(TypeXmlElementName);
            if (typeNodeCollection != null)
            {
                foreach (XmlNode typeNode in typeNodeCollection)
                {
                    var typeXmlElement = typeNode as XmlElement;
                    if (typeXmlElement == null)
                    {
                        var errorMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Require XML element but the actual is not. XML: {0}",
                            typeNode.OuterXml);
                        Trace.TraceError(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    if (typesSupportedByProvider == null)
                    {
                        typesSupportedByProvider = new Dictionary<string, IDataType>(StringComparer.OrdinalIgnoreCase);
                    }

                    var typeName = typeXmlElement.GetAttribute(TypeNameAttributeName);
                    if (string.IsNullOrEmpty(typeName))
                    {
                        var errorMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Type attribute \"{0}\" is required but the actual does not have or it is blank. XML: {1}",
                            TypeNameAttributeName,
                            typeNode.OuterXml);
                        Trace.TraceError(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    if (typesSupportedByProvider.ContainsKey(typeName))
                    {
                        var errorMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Type name has been defined. Duplicated type name found. XML: {0}",
                            typeXmlElement.OuterXml);
                        Trace.TraceError(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    var sizeName = typeXmlElement.GetAttribute(SizeNameAttributeName);
                    DataSizeType sizeType;
                    if (string.IsNullOrWhiteSpace(sizeName))
                    {
                        sizeType = DataSizeType.NotRequired;
                    }
                    else
                    {
                        sizeName = sizeName.Trim();
                        if (!DataSizeTypeConfigurationNameValueMapping.TryGetValue(sizeName, out sizeType))
                        {
                            var errorMessage = string.Format(
                                CultureInfo.InvariantCulture,
                                "Invalid size type \"{0}\". It must be empty of one of \"{1}\". XML:{2}",
                                sizeName,
                                string.Join(",", DataSizeTypeConfigurationNameValueMapping.Keys),
                                typeXmlElement.OuterXml);
                            Trace.TraceError(errorMessage);
                            throw new ArgumentException(errorMessage);
                        }
                    }

                    var dataType = new DataType(
                        typeName,
                        new DataSize(sizeType, null));
                    typesSupportedByProvider.Add(typeName, dataType);
                }
            }

            if ((typesSupportedByProvider == null) || (typesSupportedByProvider.Count < 1))
            {
                return parentTypes;
            }

            if (parentTypes != null)
            {
                foreach (var pair in parentTypes)
                {
                    var typeName = pair.Key;
                    if (!typesSupportedByProvider.ContainsKey(typeName))
                    {
                        typesSupportedByProvider.Add(typeName, pair.Value);
                    }
                }
            }

            return typesSupportedByProvider;
        }
    }
}
