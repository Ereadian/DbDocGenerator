//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DataTypeLoaderUnitTestRunner.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Test
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Data type loader unit test runner
    /// </summary>
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DataTypeLoaderUnitTestRunner : DataTypeLoaderUnitTest
    {
        /// <summary>
        /// Test passing null as XML to constructor
        /// </summary>
        [TestMethod]
        public override void ConstructorNullXml()
        {
            base.ConstructorNullXml();
        }

        /// <summary>
        /// Test constructor by pass simple types for only provider
        /// </summary>
        [TestMethod]
        public override void ConstructorSingleProvider()
        {
            base.ConstructorSingleProvider();
        }
    }
}
