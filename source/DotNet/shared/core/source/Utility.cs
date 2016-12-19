//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="Utility.cs" company="Ereadian"> 
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

    public static class Utility
    {
        public static T CreateInstanceFromTypeName<T>(string typeName, ILogger logger) where T : class
        {
            T instance = null;
            try
            {
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    var type = Type.GetType(typeName);
                    if (type != null)
                    {
                        var rawData = Activator.CreateInstance(type);
                        if (rawData != null)
                        {
                            instance = (T)rawData;
                            if (instance == null)
                            {
                                logger.Write(Events.InstanceTypeMismatch, typeof(T).FullName, rawData.GetType().FullName);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Write(Events.FailedToCreateInstanceFromConfiguration, typeof(T).FullName, exception);
            }

            return instance;
        }
    }
}
