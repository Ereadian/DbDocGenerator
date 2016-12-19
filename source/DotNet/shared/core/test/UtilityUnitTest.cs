//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="UtilityUnitTest.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using Common.Test;

    /// <summary>
    /// Unit test for default object factory
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UtilityUnitTest
    {
        #region test interfaces
        /// <summary>
        /// Simple empty interface
        /// </summary>
        public interface ISimpleForForDefaultObjectFactoryUnitTest
        {
        }

        /// <summary>
        /// Composite interface which has multiple properties
        /// </summary>
        public interface ICompositeForDefaultObjectFactoryUnitTest
        {
            /// <summary>
            /// Gets or sets integer
            /// </summary>
            int IntData { get; set; }

            /// <summary>
            /// Gets or sets string
            /// </summary>
            string StrData { get; set; }
        }

        #endregion test interfaces

        #region CreateName()
        /// <summary>
        /// Test GetName() when pass string with all valid characters
        /// </summary>
        public virtual void CreateName_AllCharactersAreValid()
        {
            var prefix = "CurrentName_0123";
            var newName = Utility.CreateName(prefix);
            Assert.IsTrue(newName.StartsWith(prefix, StringComparison.Ordinal));
            var suffix = newName.Substring(prefix.Length);
            Guid guid;
            Assert.IsTrue(Guid.TryParse(suffix, out guid));
        }

        /// <summary>
        /// Test GetName() when pass string with invalid characters
        /// </summary>
        public virtual void CreateName_WithInvalidCharacters()
        {
            var prefix = "CurrentName_0123";
            var invalidCharacters = "+- ?[]";
            var random = new Random();
            var builder = new StringBuilder();
            foreach (var ch in prefix)
            {
                builder.Append(ch);
                if (random.Next(2) == 0)
                {
                    builder.Append(invalidCharacters[random.Next(invalidCharacters.Length)]);
                }
            }

            var newName = Utility.CreateName(builder.ToString());
            Assert.IsTrue(newName.StartsWith(prefix, StringComparison.Ordinal));
            var suffix = newName.Substring(prefix.Length);
            Guid guid;
            Assert.IsTrue(Guid.TryParse(suffix, out guid));
        }
        #endregion CreateName()

        #region CreateObjectConstructor()
        /// <summary>
        /// Test CreateObjectGenerator() without base type
        /// </summary>
        public virtual void CreateObjectConstructor_Simple_NoBaseType()
        {
            var instance = CreateObject<object, ISimpleForForDefaultObjectFactoryUnitTest>();
            Assert.IsNotNull(instance);
        }

        /// <summary>
        /// Test CreateObjectGenerator() for interface with properties no base type
        /// </summary>
        public virtual void CreateObjectConstructor_Composite_NoBaseType()
        {
            var instance = CreateObject<object, ICompositeForDefaultObjectFactoryUnitTest>();
            Assert.IsNotNull(instance);
            TestInterface(instance as ICompositeForDefaultObjectFactoryUnitTest);
        }

        /// <summary>
        /// Test CreateObjectGenerator() for interface with properties has base type
        /// </summary>
        public virtual void CreateObjectConstructor_Composite_HasBaseType()
        {
            int callCount = 0;
            object instance;
            try
            {
                BaseClassForTest.ConstructorCallback = () => Interlocked.Increment(ref callCount);
                instance = CreateObject<BaseClassForTest, ICompositeForDefaultObjectFactoryUnitTest>();
            }
            finally
            {
                BaseClassForTest.ConstructorCallback = null;
            }

            Assert.IsNotNull(instance);
            Assert.AreEqual(1, callCount);
            TestInterface(instance as ICompositeForDefaultObjectFactoryUnitTest);
        }
        #endregion CreateObjectConstructor()

        #region Helper
        /// <summary>
        /// Create object
        /// </summary>
        /// <typeparam name="TBaseClass">base type</typeparam>
        /// <typeparam name="TInterface">interface type</typeparam>
        /// <returns>instance from dynamic type</returns>
        private static TInterface CreateObject<TBaseClass, TInterface>()
        {
            var type = Utility.CreateType(typeof(TBaseClass), typeof(TInterface));
            Assert.IsNotNull(type);
            var instance = Activator.CreateInstance(type);
            Assert.IsNotNull(instance);
            return (TInterface)instance;
        }

        /// <summary>
        /// Test interface
        /// </summary>
        /// <param name="instance">instance to test</param>
        private static void TestInterface(ICompositeForDefaultObjectFactoryUnitTest instance)
        {
            Assert.IsNotNull(instance);

            // Test integer
            var random = new Random();
            var intData = random.Next();
            instance.IntData = intData;
            var actualInt = instance.IntData;
            Assert.AreEqual(intData, actualInt);

            // Test string
            var strData = TestUtility.CreateRandomName("data{0}");
            instance.StrData = strData;
            var actualStr = instance.StrData;
            Assert.AreEqual(strData, actualStr);
        }

        #endregion Helper

        #region class for testing
        /// <summary>
        /// Base class for test
        /// </summary>
        [ExcludeFromCodeCoverage]
        public class BaseClassForTest
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BaseClassForTest" /> class.
            /// </summary>
            public BaseClassForTest()
            {
                if (ConstructorCallback != null)
                {
                    ConstructorCallback();
                }
            }

            /// <summary>
            /// Gets or sets the action which will be called by constructor
            /// </summary>
            public static Action ConstructorCallback { get; set; }
        }
        #endregion class for testing
    }
}
