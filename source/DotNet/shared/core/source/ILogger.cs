//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="ILogger.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// Event logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Write event
        /// </summary>
        /// <param name="id">event id</param>
        /// <param name="level">log level</param>
        /// <param name="message">log message</param>
        void Write(int id, LogLevel level, string message);
    }
}
