//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="ObjectResolverFactory.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Configuration;
    using System.Reflection;
    using System.Globalization;
    using System.Diagnostics;

    public class ObjectResolverFactory : IObjectResolverFactory
    {
        private const string FactoryTypeConfigurationKey = "ObjectResolverTypeName";

        private readonly IObjectResolverFactory instance;

        public ObjectResolverFactory()
        {
            IObjectResolverFactory factory = null;
            try
            {
                var typeName = ConfigurationManager.AppSettings[FactoryTypeConfigurationKey];
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    var type = Type.GetType(typeName);
                    if (type != null)
                    {
                        var rawData = Activator.CreateInstance(type);
                        if (rawData != null)
                        {
                            factory = (IObjectResolverFactory)rawData;
                            if (factory == null)
                            {
                                var errorMessage = string.Format(
                                    "");
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed to create {0}. Error:{1}",
                    typeof(IObjectResolverFactory).FullName,
                    exception.ToString());
                Trace.TraceError(errorMessage);
            }

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
        public IObjectResolver<T> GetResolver<T>(string resolverName = null) where T: class
        {
            return this.instance.GetResolver<T>(resolverName);
        }
    }
}
