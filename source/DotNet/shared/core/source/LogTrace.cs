//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="LogTrace.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// implement ILog by using trace
    /// </summary>
    public class LogTrace : ILogger
    {
        /// <summary>
        /// Write event
        /// </summary>
        /// <param name="id">event id</param>
        /// <param name="level">log level</param>
        /// <param name="message">log message</param>
        public void Write(int id, LogLevel level, string message)
        {
            var output = string.Format(
                CultureInfo.InvariantCulture,
                "[{0}][{1}][{2}] : {3}",
                DateTime.UtcNow,
                level,
                id,
                message);
            switch (level)
            {
                case LogLevel.Error:
                    Trace.TraceError(output);
                    break;
                case LogLevel.Warning:
                    Trace.TraceWarning(output);
                    break;
                default:
                    Trace.TraceInformation(output);
                    break;
            }
        }
    }
}
