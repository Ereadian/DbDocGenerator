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
        private ObjectCreator<object, T> creator;
        private readonly Lazy<T> instance;

        public ObjectResolver(string typeName, ILogger logger)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                this.creator = new ObjectCreator<object, T>(logger);
                this.instance = null;
            }
            else
            {
                this.creator = null;
                this.instance = new Lazy<T>(() => Utility.CreateInstanceFromTypeName<T>(typeName, logger));
            }
        }

        public T Resolve()
        {
            return this.instance == null ? this.instance.Value : this.creator.Create();
        }
    }
}
