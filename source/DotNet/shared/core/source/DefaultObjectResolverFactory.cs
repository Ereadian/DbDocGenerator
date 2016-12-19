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
            { typeof(ILogger), typeof(LogTrace)},
        };

        private IReadOnlyDictionary<Type, IReadOnlyDictionary<string, object>> resolvers;

        public DefaultObjectResolverFactory() : this(ConfigurationManager.AppSettings, Singleton<LogTrace>.Instance)
        {
        }

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
            this.resolvers = resolverCollection;
        }

        public IObjectResolver<T> GetResolver<T>(string resolverName = null) where T: class
        {
            return GetResolver(this.resolvers, typeof(T), resolverName ?? string.Empty) as IObjectResolver<T>;
        }

        public static object GetResolver(
            IReadOnlyDictionary<Type, IReadOnlyDictionary<string, object>> resolverCollection, 
            Type interfaceType, 
            string resolverName)
        {
            IReadOnlyDictionary<string, object> resolvers;
            if (resolverCollection.TryGetValue(interfaceType, out resolvers))
            {
                object resolver;
                if (resolvers.TryGetValue(resolverName, out resolver))
                {
                    return resolver;
                }
            }

            return null;
        }

        protected static object CreateResolver(Type interfaceType, string className, ILogger logger)
        {
            var resolverType = typeof(ObjectResolver<>).MakeGenericType(interfaceType);
            return Activator.CreateInstance(resolverType, className, logger);
        }

        protected static void AddResolver(
            IDictionary<Type, IReadOnlyDictionary<string, object>> resolverCollection,
            Type interfaceType,
            string resolverName,
            object resolver)
        {
            IReadOnlyDictionary<string, object> currentResolvers;
            if (!resolverCollection.TryGetValue(interfaceType, out currentResolvers))
            {
                currentResolvers = new Dictionary<string, object>();
                resolverCollection.Add(interfaceType, currentResolvers);
            }
            
            ((Dictionary<string, object>)currentResolvers)[resolverName] = resolver;
        }

        protected static void AddDefaultResolvers(
            Dictionary<Type, IReadOnlyDictionary<string, object>> resolvers, 
            IReadOnlyDictionary<Type, Type> defaultResolverTypes,
            ILogger logger)
        {
            foreach (var pair in defaultResolverTypes)
            {
                var resolver = GetResolver(resolvers, pair.Key, string.Empty);
                if (resolver == null)
                {
                    resolver = CreateResolver(pair.Key, pair.Value.AssemblyQualifiedName, logger);
                    AddResolver(resolvers, pair.Key, string.Empty, resolver);
                }
            }
        }

        protected static Tuple<string, string> GetResolverInformation(string configurationKey)
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
