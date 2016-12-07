//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DatabaseConfigurationProviderWrapper.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Test
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// Data type loader wrapper
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class DatabaseConfigurationProviderWrapper : DatabaseConfigurationProvider
    {
        /// <summary>
        /// data size type to name mapping
        /// </summary>
        private static readonly IReadOnlyDictionary<DataSizeType, string> DataSizeToConfigurationNameMapping 
            = DataSizeTypeConfigurationNameValueMapping.ToDictionary(x => x.Value, x => x.Key);

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfigurationProviderWrapper" /> class.
        /// </summary>
        /// <param name="configurationRootXml">configuration XML</param>
        internal DatabaseConfigurationProviderWrapper(XmlElement configurationRootXml) : base(configurationRootXml)
        {
        }

        /// <summary>
        /// Add type description XML element
        /// </summary>
        /// <param name="containerNode">container XML node</param>
        /// <param name="typeName">name of the type</param>
        /// <param name="sizeType">size type</param>
        /// <returns>XML element which was added</returns>
        internal static XmlElement AddTypeXml(
            XmlNode containerNode,
            string typeName,
            DataSizeType? sizeType = null)
        {
            var document = containerNode.OwnerDocument;
            var typeXmlElement = document.CreateElement(TypeXmlElementName);

            if (typeName != null)
            {
                var attribute = document.CreateAttribute(TypeNameAttributeName);
                attribute.Value = typeName;
                typeXmlElement.Attributes.Append(attribute);
            }

            if (sizeType.HasValue)
            {
                var attribute = document.CreateAttribute(SizeNameAttributeName);
                attribute.Value = DataSizeToConfigurationNameMapping[sizeType.Value];
                typeXmlElement.Attributes.Append(attribute);
            }

            containerNode.AppendChild(typeXmlElement);
            return typeXmlElement;
        }

        /// <summary>
        /// Add command XML
        /// </summary>
        /// <param name="containerNode">container XML</param>
        /// <param name="name">command name</param>
        /// <param name="data">command text</param>
        /// <param name="useCData">use CData</param>
        /// <returns>command XML</returns>
        internal static XmlElement AddCommandXml(
            XmlNode containerNode,
            string name,
            string data,
            bool useCData)
        {
            var document = containerNode.OwnerDocument;
            var commandXmlElement = document.CreateElement(CommandXmlElementName);
            containerNode.AppendChild(commandXmlElement);

            if (name != null)
            {
                var attribute = document.CreateAttribute(CommandNameAttributeName);
                attribute.Value = name;
                commandXmlElement.Attributes.Append(attribute);
            }

            XmlNode text;
            if (data != null)
            {
                if (useCData)
                {
                    text = document.CreateCDataSection(data);
                }
                else
                {
                    text = document.CreateTextNode(data);
                }

                commandXmlElement.AppendChild(text);
            }

            return commandXmlElement;
        }

        /// <summary>
        /// Add provider XML element
        /// </summary>
        /// <param name="containerNode">container XML node</param>
        /// <param name="providerName">name of database provider</param>
        /// <returns>provider XML which was added</returns>
        internal static XmlElement AddProviderXml(XmlNode containerNode, string providerName)
        {
            var document = containerNode.OwnerDocument;
            var providerXmlElement = document.CreateElement(ProviderXmlElementName);
            if (!string.IsNullOrWhiteSpace(providerName))
            {
                var attribute = document.CreateAttribute(ProviderNameAttributeName);
                attribute.Value = providerName.Trim();
                providerXmlElement.Attributes.Append(attribute);
            }

            containerNode.AppendChild(providerXmlElement);
            return providerXmlElement;
        }

        /// <summary>
        /// Create configuration root XML element
        /// </summary>
        /// <returns>root XML element</returns>
        internal static XmlElement CreateRootXml()
        {
            var document = new XmlDocument();
            return document.CreateElement("types");
        }

        /// <summary>
        /// Create data type instance
        /// </summary>
        /// <param name="name">data type name</param>
        /// <param name="type">size type</param>
        /// <param name="size">data size</param>
        /// <returns>data type instance</returns>
        internal static IDataType CreateDataType(string name, DataSizeType type, int? size = null)
        {
            return new DataType(name, type, size);
        }

        /// <summary>
        /// Create database configuration instance
        /// </summary>
        /// <param name="supportedTypes">support types</param>
        /// <param name="queryCommands">query commands</param>
        /// <returns>database configuration instance</returns>
        internal static IDatabaseConfiguration CreateDatabaseConfiguration(
                IReadOnlyDictionary<string, IDataType> supportedTypes,
                IReadOnlyDictionary<string, string> queryCommands)
        {
            return new DatabaseConfiguration(supportedTypes, queryCommands);
        }

        /// <summary>
        /// Get database configuration repository
        /// </summary>
        /// <returns>repository instance</returns>
        internal IReadOnlyDictionary<string, IDatabaseConfiguration> GetConfigurationRepository()
        {
            return this.ConfigurationRepository;
        }

        /// <summary>
        /// Data type entity
        /// </summary>
        private class DataType : IDataType
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DataType" /> class.
            /// </summary>
            /// <param name="name">type name</param>
            /// <param name="type">size type</param>
            /// <param name="size">data size</param>
            internal DataType(string name, DataSizeType type, int? size)
            {
                this.TypeName = name;
                this.DataSize = new DataSize(type, size);
            }

            /// <summary>
            /// Gets data size
            /// </summary>
            public IDataSize DataSize { get; private set; }

            /// <summary>
            /// Gets data type name
            /// </summary>
            public string TypeName { get; private set; }
        }

        /// <summary>
        /// Data size entity
        /// </summary>
        private class DataSize : IDataSize
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DataSize" /> class.
            /// </summary>
            /// <param name="sizeType">size type</param>
            /// <param name="size">data size</param>
            internal DataSize(DataSizeType sizeType, int? size)
            {
                this.Size = size;
                this.SizeType = sizeType;
            }

            /// <summary>
            /// Gets data size
            /// </summary>
            public int? Size { get; private set; }

            /// <summary>
            /// Gets size type
            /// </summary>
            public DataSizeType SizeType { get; private set; }
        }

        /// <summary>
        /// Database configuration
        /// </summary>
        private class DatabaseConfiguration : IDatabaseConfiguration
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DatabaseConfiguration" /> class.
            /// </summary>
            /// <param name="supportedTypes">support types</param>
            /// <param name="queryCommands">query commands</param>
            internal DatabaseConfiguration(
                IReadOnlyDictionary<string, IDataType> supportedTypes,
                IReadOnlyDictionary<string, string> queryCommands)
            {
                this.SupportedTypes = supportedTypes;
                this.QueryCommands = queryCommands;
            }

            /// <summary>
            /// Gets database supported types
            /// </summary>
            public IReadOnlyDictionary<string, IDataType> SupportedTypes { get; private set; }

            /// <summary>
            /// Gets query commands
            /// </summary>
            public IReadOnlyDictionary<string, string> QueryCommands { get; private set; }
        }
    }
}
