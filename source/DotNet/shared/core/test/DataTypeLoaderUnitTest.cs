//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DataTypeLoaderUnitTest.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Test
{
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
    }
}
