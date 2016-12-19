//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="UtilityUnitTestRunner.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Test
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Default object factory unit test
    /// </summary>
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class UtilityUnitTestRunner : UtilityUnitTest
    {
        #region CreateName()
        /// <summary>
        /// Test GetName() when pass string with all valid characters
        /// </summary>
        [TestMethod]
        public override void CreateName_AllCharactersAreValid()
        {
            base.CreateName_AllCharactersAreValid();
        }

        /// <summary>
        /// Test GetName() when pass string with invalid characters
        /// </summary>
        [TestMethod]
        public override void CreateName_WithInvalidCharacters()
        {
            base.CreateName_WithInvalidCharacters();
        }
        #endregion CreateName()

        #region CreateObjectGenerator()
        /// <summary>
        /// Test CreateObjectGenerator() without base type
        /// </summary>
        [TestMethod]
        public override void CreateObjectConstructor_Simple_NoBaseType()
        {
            base.CreateObjectConstructor_Simple_NoBaseType();
        }

        /// <summary>
        /// Test CreateObjectGenerator() for interface with properties no base type
        /// </summary>
        [TestMethod]
        public override void CreateObjectConstructor_Composite_NoBaseType()
        {
            base.CreateObjectConstructor_Composite_NoBaseType();
        }

        /// <summary>
        /// Test CreateObjectGenerator() for interface with properties has base type
        /// </summary>
        [TestMethod]
        public override void CreateObjectConstructor_Composite_HasBaseType()
        {
            base.CreateObjectConstructor_Composite_HasBaseType();
        }
        #endregion CreateObjectGenerator()
    }
}
