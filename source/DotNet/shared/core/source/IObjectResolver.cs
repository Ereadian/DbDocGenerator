//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IObjectResolver.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Object resolver
    /// </summary>
    /// <typeparam name="T">type of object</typeparam>
    public interface IObjectResolver<T> where T : class
    {
        /// <summary>
        /// Resolve object
        /// </summary>
        /// <returns>resolved object</returns>
        T Resolve();
    }
}
