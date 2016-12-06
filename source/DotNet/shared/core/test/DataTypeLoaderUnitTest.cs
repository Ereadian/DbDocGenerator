//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DataTypeLoaderUnitTest.cs" company="Ereadian"> 
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
    public class DataTypeLoaderUnitTest
    {
        /// <summary>
        /// Test passing null as XML to constructor
        /// </summary>
        public virtual void ConstructorNullXml()
        {
            XmlElement configurationXml = null;
            var loader = new DataTypeLoaderWrapper(configurationXml);
            var repository = loader.GetRepository();
            Assert.IsNotNull(repository);
            Assert.AreEqual(0, repository.Count);
        }

        /// <summary>
        /// Test constructor by pass simple types for only provider
        /// </summary>
        public virtual void ConstructorSingleProvider()
        {
            var possibleDataSizeTypes = EnumeratorInformation<DataSizeType>.Values;

            var expected = new Dictionary<string, IReadOnlyDictionary<string, IDataType>>(StringComparer.OrdinalIgnoreCase);
            var rootXml = DataTypeLoaderWrapper.CreateRootXml();

            var providerName = TestUtility.CreateRandomName("Provider{0}");
            var types = new Dictionary<string, IDataType>(StringComparer.OrdinalIgnoreCase);
            expected.Add(providerName, types);

            var providerXml = DataTypeLoaderWrapper.AddProviderXml(rootXml, providerName);
            string typeName;
            XmlElement typeElement;
            IDataType dataType;
            foreach (var sizeType in possibleDataSizeTypes)
            {
                typeName = TestUtility.CreateRandomName("Type{0}");
                typeElement = DataTypeLoaderWrapper.AddTypeXml(providerXml, typeName, sizeType);

                dataType = DataTypeLoaderWrapper.CreateDataType(typeName, sizeType);
                types.Add(typeName, dataType);
            }

            typeName = TestUtility.CreateRandomName("Type{0}");
            typeElement = DataTypeLoaderWrapper.AddTypeXml(providerXml, typeName);

            dataType = DataTypeLoaderWrapper.CreateDataType(typeName, DataSizeType.NotRequired);
            types.Add(typeName, dataType);

            var loader = new DataTypeLoaderWrapper(rootXml);
            var actual = loader.GetRepository();
            Assert.AreReadOnlyDictionaryEqual(
                expected,
                actual,
                (x, y) => Assert.AreReadOnlyDictionaryEqual(x, y, (a, b) => AreDataTypeEqual(a, b)));
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
