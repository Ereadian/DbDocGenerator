//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="SingletonUnitTestRunner.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Test
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Singleton unit test
    /// </summary>
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class SingletonUnitTestRunner : SingletonUnitTest
    {
        /// <summary>
        /// Create singleton successfully
        /// </summary>
        [TestMethod]
        public override void SingletonCreateSuccess()
        {
            base.SingletonCreateSuccess();
        }

        /// <summary>
        /// Create singleton failure
        /// </summary>
        [TestMethod]
        public override void SingletonCreateFailure()
        {
            base.SingletonCreateFailure();
        }
    }
}
