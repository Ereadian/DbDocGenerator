//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DatabaseConfigurationProviderUnitTest.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
    using Common.Test;

    /// <summary>
    /// Data type loader unit test
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DatabaseConfigurationProviderUnitTest
    {
        /// <summary>
        /// Test passing null as XML to constructor
        /// </summary>
        public virtual void ConstructorNullXml()
        {
            XmlElement configurationXml = null;
            var loader = new DatabaseConfigurationProviderWrapper(configurationXml);
            var repository = loader.GetConfigurationRepository();
            Assert.IsNotNull(repository);
            Assert.AreEqual(0, repository.Count);
        }

        /// <summary>
        /// Test constructor by pass simple types for only provider
        /// </summary>
        public virtual void ConstructorSingleProvider()
        {
            var possibleDataSizeTypes = EnumeratorInformation<DataSizeType>.Values;

            var expected = new Dictionary<string, IDatabaseConfiguration>(StringComparer.OrdinalIgnoreCase);
            var rootXml = DatabaseConfigurationProviderWrapper.CreateRootXml();

            var providerName = TestUtility.CreateRandomName("Provider{0}");
            var types = new Dictionary<string, IDataType>(StringComparer.OrdinalIgnoreCase);
            var commands = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var configuration = DatabaseConfigurationProviderWrapper.CreateDatabaseConfiguration(types, commands);
            expected.Add(providerName, configuration);

            var providerXml = DatabaseConfigurationProviderWrapper.AddProviderXml(rootXml, providerName);

            // Add type XML
            string typeName;
            IDataType dataType;
            foreach (var sizeType in possibleDataSizeTypes)
            {
                typeName = TestUtility.CreateRandomName("Type{0}");
                DatabaseConfigurationProviderWrapper.AddTypeXml(providerXml, typeName, sizeType);

                dataType = DatabaseConfigurationProviderWrapper.CreateDataType(typeName, sizeType);
                types.Add(typeName, dataType);
            }

            typeName = TestUtility.CreateRandomName("Type{0}");
            DatabaseConfigurationProviderWrapper.AddTypeXml(providerXml, typeName);

            dataType = DatabaseConfigurationProviderWrapper.CreateDataType(typeName, DataSizeType.NotRequired);
            types.Add(typeName, dataType);

            // Add command XML
            int commandCount = 3;
            for (var i = 0; i < commandCount; i++)
            {
                var commandName = TestUtility.CreateRandomName("name{0}");
                var commandText = TestUtility.CreateRandomName("text{0}");
                var commandXml = DatabaseConfigurationProviderWrapper.AddCommandXml(providerXml, commandName, commandText, i % 2 == 0);
                commands.Add(commandName, commandText);
            }

            var loader = new DatabaseConfigurationProviderWrapper(rootXml);
            var actual = loader.GetConfigurationRepository();
            Assert.AreReadOnlyDictionaryEqual(
                expected,
                actual,
                (x, y) =>
                {
                    Assert.AreReadOnlyDictionaryEqual(x.QueryCommands, y.QueryCommands);
                    Assert.AreReadOnlyDictionaryEqual(
                        x.SupportedTypes, 
                        y.SupportedTypes,
                        (a, b) =>
                        {
                            Assert.AreEqual(a.TypeName, b.TypeName);
                            Assert.AreEqual(a.DataSize.SizeType, b.DataSize.SizeType);
                            Assert.AreEqual(a.DataSize.Size, b.DataSize.Size);
                        });
                });
        }

        /// <summary>
        /// Helper function to check two data type and expect they are same
        /// </summary>
        /// <param name="expected">expected data type</param>
        /// <param name="actual">actual data type</param>
        private static void AreDataTypeEqual(IDataType expected, IDataType actual)
        {
            Assert.AreEqual(expected.TypeName, actual.TypeName);
            Assert.AreEqual(expected.DataSize.SizeType, actual.DataSize.SizeType);
            Assert.AreEqual(expected.DataSize.Size, actual.DataSize.Size);
        }
    }
}
