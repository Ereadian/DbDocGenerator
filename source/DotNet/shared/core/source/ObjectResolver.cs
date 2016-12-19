//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="ObjectResolver.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;

    /// <summary>
    /// Object resolver
    /// </summary>
    /// <typeparam name="T">interface type</typeparam>
    public class ObjectResolver<T> : IObjectResolver<T> where T : class
    {
        /// <summary>
        /// dynamic object creator
        /// </summary>
        private readonly ObjectCreator<object, T> creator;

        /// <summary>
        /// Singleton object creator
        /// </summary>
        private readonly Lazy<T> instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectResolver{T}" /> class.
        /// </summary>
        /// <param name="typeName">target type name</param>
        /// <param name="logger">event logger</param>
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

        /// <summary>
        /// Resolve object
        /// </summary>
        /// <returns>resolved object</returns>
        public T Resolve()
        {
            return this.instance == null ? this.instance.Value : this.creator.Create();
        }
    }
}
