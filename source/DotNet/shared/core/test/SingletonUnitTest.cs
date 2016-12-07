//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="SingletonUnitTest.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Common.Test;

    /// <summary>
    /// Singleton unit test
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SingletonUnitTest
    {
        /// <summary>
        /// Create singleton successfully
        /// </summary>
        public virtual void SingletonCreateSuccess()
        {
            var instance = Singleton<SingletonTestClassForSuccess>.Instance;
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance is SingletonTestClassForSuccess);

            var another = Singleton<SingletonTestClassForSuccess>.Instance;
            Assert.IsNotNull(another);
            Assert.IsTrue(object.ReferenceEquals(instance, another));
        }

        /// <summary>
        /// Create singleton failure
        /// </summary>
        public virtual void SingletonCreateFailure()
        {
            var instance = Singleton<SingletonTestClassForFailure>.Instance;
            Assert.IsNull(instance);
        }

        /// <summary>
        /// Test class for success scenario
        /// </summary>
        public class SingletonTestClassForSuccess
        {
        }

        /// <summary>
        /// Test class for failure scenario
        /// </summary>
        public class SingletonTestClassForFailure
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SingletonTestClassForFailure" /> class.
            /// </summary>
            public SingletonTestClassForFailure()
            {
                throw new ApplicationException();
            }
        }
    }
}
