//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="Events.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// events for logging
    /// </summary>
    public enum Events
    {
        /// <summary>
        /// Failed to create instance from configuration
        /// </summary>
        [Event("Failed to create instance for type {0} from configuration. Exception:{1}")]
        FailedToCreateInstanceFromConfiguration = 1000,

        /// <summary>
        /// Instance type does not match
        /// </summary>
        [Event("Trying to create instance for type {0}. However, got {1} which is not compatible")]
        InstanceTypeMismatch,

        /// <summary>
        /// Could not find type for interface during resolver factory initizaliation
        /// </summary>
        [Event("Could not load type for interface \"{0}\"")]
        InterfaceTypeDoesNotExist,
    }
}
