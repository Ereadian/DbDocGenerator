//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DataTypeLoaderUnitTestRunner.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Test
{
	using NUnit.Framework;
	using System;

    /// <summary>
    /// Data type loader unit test runner.
    /// </summary>
	[TestFixture]
	public class DataTypeLoaderUnitTestRunner : DataTypeLoaderUnitTest
	{
        /// <summary>
        /// Test passing null as XML to constructor
        /// </summary>
		[Test]
		public override void ConstructorNullXml ()
		{
			base.ConstructorNullXml ();
		}

        /// <summary>
        /// Test constructor by pass simple types for only one provider
        /// </summary>
        [Test]
        public override void ConstructorSingleProvider ()
        {
            base.ConstructorSingleProvider ();
        }
    }
}
