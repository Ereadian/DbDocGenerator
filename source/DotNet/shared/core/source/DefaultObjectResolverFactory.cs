//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DefaultObjectResolverFactory.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;

    /// <summary>
    /// Default object resolver factory
    /// </summary>
    public class DefaultObjectResolverFactory : IObjectResolverFactory
    {
        /// <summary>
        /// configuration key prefix
        /// </summary>
        public const string ResolverConfiguraitonKeyPrefix = "[Resolver";

        /// <summary>
        /// name start character
        /// </summary>
        public const char ResolverNameStartCharacter = ':';

        /// <summary>
        /// prefix end character
        /// </summary>
        public const char ReslverPrefixEndChar = ']';

        /// <summary>
        /// Default resolvers
        /// </summary>
        protected static readonly IReadOnlyDictionary<Type, Type> DefaultResolvers = new Dictionary<Type, Type>()
        {
            { typeof(ILogger), typeof(LogTrace) },
            { typeof(IDatabaseConfigurationProvider), typeof(DatabaseConfigurationProvider) },
            { typeof(IDatabaseAnalysisResult), null },
            { typeof(IFormatter), typeof(Formater.HtmlFormatter) },
            { typeof(ITable), null },
            { typeof(IColumn), null },
            { typeof(IConstraint), null },
            { typeof(IRoutine), null },
            { typeof(IParameter), null },
            { typeof(IIndex), null },
            { typeof(IReference), null }
        };

        /// <summary>
        /// Resolver repository
        /// </summary>
        private IReadOnlyDictionary<Type, IReadOnlyDictionary<string, object>> resolverRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultObjectResolverFactory" /> class.
        /// </summary>
        public DefaultObjectResolverFactory() : this(ConfigurationManager.AppSettings, Singleton<LogTrace>.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultObjectResolverFactory" /> class.
        /// </summary>
        /// <param name="configurations">configuration data</param>
        /// <param name="logger">event logger</param>
        public DefaultObjectResolverFactory(NameValueCollection configurations, ILogger logger)
        {
            var resolverCollection = new Dictionary<Type, IReadOnlyDictionary<string, object>>();
            if (configurations != null)
            {
                var configurationKeys = configurations.AllKeys;
                foreach (var key in configurationKeys)
                {
                    var resovlerInformation = GetResolverInformation(key);
                    if (resovlerInformation != null)
                    {
                        var resolverName = resovlerInformation.Item1;
                        var interfaceTypeName = resovlerInformation.Item2;
                        var interfaceType = Type.GetType(interfaceTypeName);
                        if (interfaceType == null)
                        {
                            logger.Write(Events.InterfaceTypeDoesNotExist, interfaceTypeName);
                        }
                        else
                        {
                            var resolver = CreateResolver(interfaceType, configurations[key], logger);
                            AddResolver(resolverCollection, interfaceType, resolverName, resolver);
                        }
                    }
                }
            }

            AddDefaultResolvers(resolverCollection, DefaultResolvers, logger);
            this.resolverRepository = resolverCollection;
        }

        /// <summary>
        /// Get object resolver
        /// </summary>
        /// <typeparam name="T">type to resolve</typeparam>
        /// <param name="resolverName">resolver name</param>
        /// <returns>resolver instance</returns>
        public IObjectResolver<T> GetResolver<T>(string resolverName = null) where T : class
        {
            return GetResolver(this.resolverRepository, typeof(T), resolverName ?? string.Empty) as IObjectResolver<T>;
        }

        /// <summary>
        /// Get resolver helper
        /// </summary>
        /// <param name="repository">resolver repository</param>
        /// <param name="interfaceType">type of interface</param>
        /// <param name="resolverName">resolver name</param>
        /// <returns>resolver instance</returns>
        private static object GetResolver(
            IReadOnlyDictionary<Type, IReadOnlyDictionary<string, object>> repository, 
            Type interfaceType, 
            string resolverName)
        {
            IReadOnlyDictionary<string, object> resolvers;
            if (repository.TryGetValue(interfaceType, out resolvers))
            {
                object resolver;
                if (resolvers.TryGetValue(resolverName, out resolver))
                {
                    return resolver;
                }
            }

            return null;
        }

        /// <summary>
        /// Create new resolver
        /// </summary>
        /// <param name="interfaceType">interface type</param>
        /// <param name="className">target instance class name</param>
        /// <param name="logger">event logger</param>
        /// <returns>resolver instance</returns>
        private static object CreateResolver(Type interfaceType, string className, ILogger logger)
        {
            var resolverType = typeof(ObjectResolver<>).MakeGenericType(interfaceType);
            return Activator.CreateInstance(resolverType, className, logger);
        }

        /// <summary>
        /// Add resolver to repository
        /// </summary>
        /// <param name="repository">resolver repository</param>
        /// <param name="interfaceType">interface type</param>
        /// <param name="resolverName">resolver name</param>
        /// <param name="resolver">resolver instance</param>
        private static void AddResolver(
            IDictionary<Type, IReadOnlyDictionary<string, object>> repository,
            Type interfaceType,
            string resolverName,
            object resolver)
        {
            IReadOnlyDictionary<string, object> currentResolvers;
            if (!repository.TryGetValue(interfaceType, out currentResolvers))
            {
                currentResolvers = new Dictionary<string, object>();
                repository.Add(interfaceType, currentResolvers);
            }
            
            ((Dictionary<string, object>)currentResolvers)[resolverName] = resolver;
        }

        /// <summary>
        /// Add default resolver into resolver repository if they have not been defined
        /// </summary>
        /// <param name="respository">resolver repository</param>
        /// <param name="defaultResolverTypes">default types</param>
        /// <param name="logger">event logger</param>
        private static void AddDefaultResolvers(
            Dictionary<Type, IReadOnlyDictionary<string, object>> respository, 
            IReadOnlyDictionary<Type, Type> defaultResolverTypes,
            ILogger logger)
        {
            foreach (var pair in defaultResolverTypes)
            {
                var resolver = GetResolver(respository, pair.Key, string.Empty);
                if (resolver == null)
                {
                    string typeName = pair.Value != null ? pair.Value.AssemblyQualifiedName : null;
                    resolver = CreateResolver(pair.Key, typeName, logger);
                    AddResolver(respository, pair.Key, string.Empty, resolver);
                }
            }
        }

        /// <summary>
        /// Get resolver information from configuration key
        /// </summary>
        /// <param name="configurationKey">configuration key</param>
        /// <returns>resolver information</returns>
        private static Tuple<string, string> GetResolverInformation(string configurationKey)
        {
            if (!configurationKey.StartsWith(ResolverConfiguraitonKeyPrefix, StringComparison.Ordinal)
                || (configurationKey.Length <= (ResolverConfiguraitonKeyPrefix.Length + 1)))
            {
                return null;
            }

            var resolverHeaderEndPosition = configurationKey.IndexOf(ReslverPrefixEndChar);
            if (resolverHeaderEndPosition < 0)
            {
                return null;
            }

            var nameStartPosition = configurationKey.IndexOf(ResolverNameStartCharacter);
            string resolverName = string.Empty;
            if ((nameStartPosition > 0) && (nameStartPosition < resolverHeaderEndPosition))
            {
                var length = resolverHeaderEndPosition - nameStartPosition - 1;
                if (length > 0)
                {
                    resolverName = configurationKey.Substring(nameStartPosition + 1, length).Trim();
                }
            }

            var interfaceTypeName = configurationKey.Substring(resolverHeaderEndPosition + 1).Trim();
            return new Tuple<string, string>(resolverName, interfaceTypeName);
        }
    }
}
