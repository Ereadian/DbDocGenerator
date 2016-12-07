//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DatabaseConfigurationProvider.cs" company="Ereadian"> 
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
    public class DatabaseConfigurationProvider : IDatabaseConfigurationProvider
    {
        #region XML names
        /// <summary>
        /// XML generic name attribute name
        /// </summary>
        protected const string NameAttributeName = "name";

        /// <summary>
        /// Provider XML element name
        /// </summary>
        protected const string ProviderXmlElementName = "provider";

        /// <summary>
        /// Provider name attribute
        /// </summary>
        protected const string ProviderNameAttributeName = NameAttributeName;

        /// <summary>
        /// Type XML element name
        /// </summary>
        protected const string TypeXmlElementName = "type";

        /// <summary>
        /// Type name attribute
        /// </summary>
        protected const string TypeNameAttributeName = NameAttributeName;

        /// <summary>
        /// Type size attribute
        /// </summary>
        protected const string SizeNameAttributeName = "size";

        /// <summary>
        /// Command XML element name
        /// </summary>
        protected const string CommandXmlElementName = "command";

        /// <summary>
        /// Command name attribute
        /// </summary>
        protected const string CommandNameAttributeName = NameAttributeName;
        #endregion XML names

        #region Size attribute values
        /// <summary>
        /// size is not required
        /// </summary>
        protected const string DataSizeTypeConfigurationNameForNotRequired = "no";

        /// <summary>
        /// Size is required
        /// </summary>
        protected const string DataSizeTypeConfigurationNameForRequired = "required";

        /// <summary>
        /// Size is required and can apply "MAX"
        /// </summary>
        protected const string DataSizeTypeConfigurationNameForAllowMax = "maximum";
        #endregion Size attribute values

        /// <summary>
        /// Configuration XML filename
        /// </summary>
        protected const string TypesConfigurationXmlFilename = "configurations.xml";

        /// <summary>
        /// Gets data size name-type mapping 
        /// </summary>
        protected static readonly IReadOnlyDictionary<string, DataSizeType> DataSizeTypeConfigurationNameValueMapping
            = new Dictionary<string, DataSizeType>(StringComparer.OrdinalIgnoreCase)
            {
                { DataSizeTypeConfigurationNameForNotRequired, DataSizeType.NotRequired },
                { DataSizeTypeConfigurationNameForRequired, DataSizeType.Required },
                { DataSizeTypeConfigurationNameForAllowMax, DataSizeType.Maximum },
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfigurationProvider" /> class.
        /// </summary>
        public DatabaseConfigurationProvider() : this(LoadConfigurationXml(TypesConfigurationXmlFilename))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfigurationProvider" /> class.
        /// </summary>
        /// <param name="configurationRootXml">configuration XML</param>
        public DatabaseConfigurationProvider(XmlElement configurationRootXml)
        {
            var repository = new Dictionary<string, IDatabaseConfiguration>(StringComparer.OrdinalIgnoreCase);
            this.ConfigurationRepository = repository;
            if (configurationRootXml != null)
            {
                LoadProviders(repository, configurationRootXml, null, null);
            }
        }

        /// <summary>
        /// Gets all database provider names
        /// </summary>
        public IReadOnlyCollection<string> SupportedDatabaseProviderNames { get; private set; }

        /// <summary>
        /// Gets types per provider repository
        /// </summary>
        protected IReadOnlyDictionary<string, IDatabaseConfiguration> ConfigurationRepository { get; private set; }

        /// <summary>
        /// Gets database provider configuration by given provider name
        /// </summary>
        /// <param name="databaseProviderName">database provider name</param>
        /// <returns>database provider configuration</returns>
        public IDatabaseConfiguration this[string databaseProviderName]
        {
            get
            {
                IDatabaseConfiguration configuration;
                return this.ConfigurationRepository.TryGetValue(databaseProviderName, out configuration) ? configuration : null;
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
            var type = typeof(DatabaseConfigurationProvider);
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
        /// <param name="parentCommands">parent commands</param>
        private static void LoadProviders(
            IDictionary<string, IDatabaseConfiguration> repository,
            XmlElement providerXml,
            IReadOnlyDictionary<string, IDataType> parentTypes,
            IReadOnlyDictionary<string, string> parentCommands)
        {
            var types = LoadTypes(providerXml, parentTypes);
            var commands = LoadCommands(providerXml, parentCommands);
            IDatabaseConfiguration configuration = null;
            if ((types != null) || (commands != null))
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

                    configuration = new DatabaseConfiguration(types, commands);
                    repository.Add(providerName, configuration);
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

                    LoadProviders(repository, inheritedProviderXml, types, commands);
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
            Dictionary<string, IDataType> supportedTypes = null;
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

                    if (supportedTypes == null)
                    {
                        supportedTypes = new Dictionary<string, IDataType>(StringComparer.OrdinalIgnoreCase);
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

                    if (supportedTypes.ContainsKey(typeName))
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
                    supportedTypes.Add(typeName, dataType);
                }
            }

            if ((supportedTypes == null) || (supportedTypes.Count < 1))
            {
                return parentTypes;
            }

            supportedTypes.AppendFromReadOnly(parentTypes);
            return supportedTypes;
        }

        /// <summary>
        /// Load commands for current provider
        /// </summary>
        /// <param name="providerXml">current provider XML</param>
        /// <param name="parentCommands">parent commands</param>
        /// <returns>commands for current provider</returns>
        private static IReadOnlyDictionary<string, string> LoadCommands(
            XmlElement providerXml,
            IReadOnlyDictionary<string, string> parentCommands)
        {
            Dictionary<string, string> commands = null;
            var commandNodeCollection = providerXml.SelectNodes(CommandXmlElementName);
            if (commandNodeCollection != null)
            {
                foreach (XmlNode commandNode in commandNodeCollection)
                {
                    var commandXmlElement = commandNode as XmlElement;
                    if (commandXmlElement == null)
                    {
                        var errorMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Require command XML element but the actual is not. XML: {0}",
                            commandNode.OuterXml);
                        Trace.TraceError(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    if (commands == null)
                    {
                        commands = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    }

                    var commandName = commandXmlElement.GetAttribute(CommandNameAttributeName);
                    if (string.IsNullOrEmpty(commandName))
                    {
                        var errorMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Command attribute \"{0}\" is required but the actual does not have or it is blank. XML: {1}",
                            CommandNameAttributeName,
                            commandNode.OuterXml);
                        Trace.TraceError(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    if (commands.ContainsKey(commandName))
                    {
                        var errorMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Command \"{0}\" has been defined. Duplicated command name found. XML: {1}",
                            commandName,
                            commandXmlElement.OuterXml);
                        Trace.TraceError(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    var commandText = commandXmlElement.InnerText;
                    if (string.IsNullOrWhiteSpace(commandText))
                    {
                        var errorMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Command \"{0}\" does not have valid content. XML: {1}",
                            commandName,
                            commandXmlElement.OuterXml);
                        Trace.TraceError(errorMessage);
                        throw new ArgumentException(errorMessage);
                    }

                    commands.Add(commandName, commandText);
                }
            }

            if ((commands == null) || (commands.Count < 1))
            {
                return parentCommands;
            }

            commands.AppendFromReadOnly(parentCommands);
            return commands;
        }
    }
}
