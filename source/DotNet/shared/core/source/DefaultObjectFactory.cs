//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DefaultObjectFactory.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Object factory default implementation
    /// </summary>
    /// <typeparam name="TBaseClass">type of base class</typeparam>
    /// <typeparam name="TInterface">Type of the object to create. It should be an interface</typeparam>
    public class DefaultObjectFactory<TBaseClass, TInterface> : IObjectFactory<TBaseClass, TInterface>
        where TBaseClass : class, new()
        where TInterface : class
    {
        /// <summary>
        /// Constructor information
        /// </summary>
        private readonly ConstructorInfo TypeConstructorInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultObjectFactory{TBaseClass, TInterface}" /> class.
        /// </summary>
        public DefaultObjectFactory()
        {
            try
            {
                TypeConstructorInformation = CreateObjectConstructor(typeof(TBaseClass), typeof(TInterface));
            }
            catch (Exception exception)
            {
                TypeConstructorInformation = null;
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed to create constructor while implementing interface {0}. Exception:{1}",
                    typeof(TInterface).FullName,
                    exception.ToString());
                Trace.TraceError(errorMessage);
            }
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <returns>new instance which supports the interface</returns>
        public virtual TInterface Create()
        {
            return TypeConstructorInformation == null ? null : (TInterface)TypeConstructorInformation.Invoke(null);
        }

        /// <summary>
        /// Create a unique name
        /// </summary>
        /// <param name="prefix">name prefix</param>
        /// <returns>random unique name</returns>
        protected static string CreateName(string prefix)
        {
            return Utility.CreateName(prefix);
        }

        /// <summary>
        /// Create type and return default constructor information
        /// </summary>
        /// <param name="baseType">base type</param>
        /// <param name="interfaceType">type of the interface to implement</param>
        /// <returns>constructor information</returns>
        protected static ConstructorInfo CreateObjectConstructor(Type baseType, Type interfaceType)
        {
            var assemblyBuilder = Utility.CreateAssemblyBuilder(interfaceType);
            var moduleBuilder = Utility.CreateModuleBuilder(assemblyBuilder, interfaceType);
            var typeBuilder = Utility.CreateTypeBuilder(moduleBuilder, baseType, interfaceType);
            var constructorBuilder = Utility.CreateDefaultConstructor(typeBuilder, baseType, interfaceType);
            Utility.CreateProperties(typeBuilder, interfaceType);
            var type = typeBuilder.CreateType();
            return type.GetConstructor(Type.EmptyTypes);
        }
    }
}
