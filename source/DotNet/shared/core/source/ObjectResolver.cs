//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="ObjectResolver.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;

    public class ObjectResolver<T> : IObjectResolver<T> where T : class
    {
        private readonly Lazy<T> instance;

        public ObjectResolver(string typeName, ILogger logger)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                instance = new Lazy<T>(() => Singleton<ObjectFactory>.Instance.Create<object, T>());
            }
            else
            {
                instance = new Lazy<T>(() => Utility.CreateInstanceFromTypeName<T>(typeName, logger));
            }
        }

        public T Resolve()
        {
            return this.instance.Value;
        }
    }
}
