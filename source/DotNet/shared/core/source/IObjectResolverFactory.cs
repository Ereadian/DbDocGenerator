//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="ObjectResolver.cs" company="Ereadian"> 
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

    public interface IObjectResolverFactory
    {
        /// <summary>
        /// Get object resolver
        /// </summary>
        /// <typeparam name="T">type to resolve</typeparam>
        /// <param name="resolverName">resolver name</param>
        /// <returns>resolver instance</returns>
        IObjectResolver<T> GetResolver<T>(string resolverName = null) where T: class;
    }
}
