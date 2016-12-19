//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="ObjectFactory.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Object factory
    /// </summary>
    public static class ObjectFactory
    {
        /// <summary>
        /// Create class for a given interface
        /// </summary>
        /// <typeparam name="TInterface">type of the interface to implement</typeparam>
        /// <returns>instance which implements the interface</returns>
        public static TInterface Create<TInterface>() where TInterface : class
        {
            return Create<object, TInterface>();
        }

        /// <summary>
        /// Create class for a given base class and interface
        /// </summary>
        /// <typeparam name="TBaseClass">type of base class</typeparam>
        /// <typeparam name="TInterface">type of the interface to implement</typeparam>
        /// <returns>instance which implements the interface</returns>
        public static TInterface Create<TBaseClass, TInterface>() where TBaseClass : class, new() where TInterface : class
        {
            return Singleton<DefaultObjectFactory<TBaseClass, TInterface>>.Instance.Create();
        }
    }
}
