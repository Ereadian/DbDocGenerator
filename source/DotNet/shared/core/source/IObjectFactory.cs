//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IObjectFactory.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Object factory
    /// </summary>
    /// <typeparam name="TBaseClass">type of base class</typeparam>
    /// <typeparam name="TInterface">Type of the object to create. It should be an interface</typeparam>
    /// <remarks>
    /// This factory accepts a interface and create internal class for that. When creating instance, the factory create a new internal
    /// instance and return it. This will avoid defining too many classes without taking any advantage.
    /// </remarks>
    public interface IObjectFactory<TBaseClass, TInterface>
        where TBaseClass : class
        where TInterface : class
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <returns>new instance which supports the interface</returns>
        TInterface Create();
    }
}
