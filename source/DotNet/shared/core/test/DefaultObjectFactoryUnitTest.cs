//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DefaultObjectFactoryUnitTest.cs" company="Ereadian"> 
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
    public class DefaultObjectFactoryUnitTest
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
            var newName = ObjectFactoryForTest<object, ISimpleForForDefaultObjectFactoryUnitTest>.CreateNameForTest(prefix);
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

            var newName = ObjectFactoryForTest<object, ISimpleForForDefaultObjectFactoryUnitTest>.CreateNameForTest(builder.ToString());
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
            var constructorInformation = ObjectFactoryForTest<object, ISimpleForForDefaultObjectFactoryUnitTest>
                .CreateObjectGeneratorForTest();
            var instance = constructorInformation.Invoke(null);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance is ISimpleForForDefaultObjectFactoryUnitTest);
        }

        /// <summary>
        /// Test CreateObjectGenerator() for interface with properties no base type
        /// </summary>
        public virtual void CreateObjectConstructor_Composite_NoBaseType()
        {
            var constructorInformation = ObjectFactoryForTest<object, ISimpleForForDefaultObjectFactoryUnitTest>
                .CreateObjectGeneratorForTest(typeof(object), typeof(ICompositeForDefaultObjectFactoryUnitTest));
            var instance = constructorInformation.Invoke(null);
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
                var constructorInformation = ObjectFactoryForTest<object, ISimpleForForDefaultObjectFactoryUnitTest>
                    .CreateObjectGeneratorForTest(typeof(BaseClassForTest), typeof(ICompositeForDefaultObjectFactoryUnitTest));
                instance = constructorInformation.Invoke(null);
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

        #region DefaultObjectFactoryWrapper
        /// <summary>
        /// Wrapper for default object factory
        /// </summary>
        /// <typeparam name="TBase">type of base class</typeparam>
        /// <typeparam name="TInterface">type of interface</typeparam>
        [ExcludeFromCodeCoverage]
        public class ObjectFactoryForTest<TBase, TInterface> : DefaultObjectFactory<TBase, TInterface>
            where TBase : class, new()
            where TInterface : class
        {
            /// <summary>
            /// Wrapper for creating random name
            /// </summary>
            /// <param name="prefix">name prefix</param>
            /// <returns>random name</returns>
            internal static string CreateNameForTest(string prefix)
            {
                return ObjectFactoryForTest<TBase, TInterface>.CreateName(prefix);
            }

            /// <summary>
            /// Wrapper for creating object constructor
            /// </summary>
            /// <param name="baseType">base type</param>
            /// <param name="interfaceType">type of interface</param>
            /// <returns>constructor information</returns>
            internal static ConstructorInfo CreateObjectGeneratorForTest(
                Type baseType = null, 
                Type interfaceType = null)
            {
                if (baseType == null)
                {
                    baseType = typeof(object);
                }

                if (interfaceType == null)
                {
                    interfaceType = typeof(TInterface);
                }

                return ObjectFactoryForTest<TBase, TInterface>.CreateObjectConstructor(baseType, interfaceType);
            }
        }
        #endregion DefaultObjectFactoryWrapper
    }
}
