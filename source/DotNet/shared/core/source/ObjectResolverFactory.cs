//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="ObjectResolverFactory.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Configuration;

    /// <summary>
    /// Object resolver factory
    /// </summary>
    public class ObjectResolverFactory : IObjectResolverFactory
    {
        /// <summary>
        /// resolver type name configuration key
        /// </summary>
        private const string FactoryTypeConfigurationKey = "ObjectResolverTypeName";

        /// <summary>
        /// current resolver factory instance
        /// </summary>
        private readonly IObjectResolverFactory instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectResolverFactory" /> class.
        /// </summary>
        public ObjectResolverFactory()
        {
            var typeName = ConfigurationManager.AppSettings[FactoryTypeConfigurationKey];
            var factory = Utility.CreateInstanceFromTypeName<IObjectResolverFactory>(typeName, Singleton<LogTrace>.Instance);
            if (factory == null)
            {
                factory = new DefaultObjectResolverFactory();
            }

            this.instance = factory;
        }

        /// <summary>
        /// Get object resolver
        /// </summary>
        /// <typeparam name="T">type to resolve</typeparam>
        /// <param name="resolverName">resolver name</param>
        /// <returns>resolver instance</returns>
        public IObjectResolver<T> GetResolver<T>(string resolverName = null) where T : class
        {
            return this.instance.GetResolver<T>(resolverName);
        }
    }
}
