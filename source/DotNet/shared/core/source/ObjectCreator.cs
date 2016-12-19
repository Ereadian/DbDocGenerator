//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="ObjectCreator.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Object factory default implementation
    /// </summary>
    /// <typeparam name="TBaseClass">type of base class</typeparam>
    /// <typeparam name="TInterface">Type of the object to create. It should be an interface</typeparam>
    public class ObjectCreator<TBaseClass, TInterface>
        where TBaseClass : class, new()
        where TInterface : class
    {
        /// <summary>
        /// Constructor information
        /// </summary>
        private readonly ConstructorInfo typeConstructorInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectCreator{TBaseClass, TInterface}" /> class.
        /// </summary>
        /// <param name="logger">event logger</param>
        public ObjectCreator(ILogger logger)
        {
            try
            {
                var type = Utility.CreateType(typeof(TBaseClass), typeof(TInterface));
                this.typeConstructorInformation = type.GetConstructor(Type.EmptyTypes);
            }
            catch (Exception exception)
            {
                this.typeConstructorInformation = null;
                logger.Write(
                    Events.FailedToCreateDynamicType, 
                    typeof(TBaseClass).FullName, 
                    typeof(TInterface).FullName, 
                    exception.ToString());
            }
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <returns>new instance which supports the interface</returns>
        public virtual TInterface Create()
        {
            return this.typeConstructorInformation == null ? null : (TInterface)this.typeConstructorInformation.Invoke(null);
        }
    }
}
