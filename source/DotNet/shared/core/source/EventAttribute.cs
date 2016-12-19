//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="EventAttribute.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;

    /// <summary>
    /// Event attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EventAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventAttribute" /> class.
        /// </summary>
        /// <param name="messageTemplate">message template</param>
        public EventAttribute(string messageTemplate = null) : this(LogLevel.Error, messageTemplate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAttribute" /> class.
        /// </summary>
        /// <param name="level">log level</param>
        /// <param name="messageTemplate">message template</param>
        public EventAttribute(LogLevel level, string messageTemplate)
        {
            this.Level = level;
            this.MessageTemplate = messageTemplate;
        }

        /// <summary>
        /// Gets log level
        /// </summary>
        public LogLevel Level { get; private set; }

        /// <summary>
        /// Gets message template
        /// </summary>
        public string MessageTemplate { get; private set; }
    }
}
