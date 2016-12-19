//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="LogExtension.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// ILog extension
    /// </summary>
    public static class LogExtension
    {
        /// <summary>
        /// Write log
        /// </summary>
        /// <typeparam name="T">type of log event enumerator</typeparam>
        /// <param name="logger">event log instance</param>
        /// <param name="id">event id</param>
        /// <param name="parameters">parameters for build message</param>
        public static void Write<T>(this ILogger logger, T id, params object[] parameters)
        {
            var eventItemInformation = Singleton<EventInformation<T>>.Instance[id];
            string errorMessage;
            if (parameters.IsNullOrEmpty())
            {
                errorMessage = eventItemInformation.MessageTemplate;
            }
            else
            {
                errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    eventItemInformation.MessageTemplate,
                    parameters);
            }

            logger.Write(eventItemInformation.Id, eventItemInformation.Level, errorMessage);
        }

        /// <summary>
        /// Event information
        /// </summary>
        /// <typeparam name="T">type of event enumerator</typeparam>
        public class EventInformation<T>
        {
            /// <summary>
            /// item information collection
            /// </summary>
            private readonly IReadOnlyDictionary<T, EventItemInformation> eventItems;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventInformation{T}" /> class.
            /// </summary>
            public EventInformation()
            {
                var items = new Dictionary<T, EventItemInformation>();
                this.eventItems = items;
                var type = typeof(T);
                var names = Enum.GetNames(type) as string[];
                foreach (var name in names)
                {
                    var fieldInformation = type.GetField(name);
                    if (fieldInformation != null)
                    {
                        var attribute = fieldInformation.GetCustomAttribute<EventAttribute>();
                        if (attribute != null)
                        {
                            var id = fieldInformation.GetValue(null);
                            var item = new EventItemInformation((int)id, attribute.Level, attribute.MessageTemplate);
                            items.Add((T)id, item);
                        }
                    }
                }
            }

            /// <summary>
            /// Gets event item information by id
            /// </summary>
            /// <param name="id">event id</param>
            /// <returns>event item information</returns>
            internal EventItemInformation this[T id]
            {
                get
                {
                    EventItemInformation item;
                    return this.eventItems.TryGetValue(id, out item) ? item : null;
                }
            }
        }

        /// <summary>
        /// Event item information
        /// </summary>
        internal class EventItemInformation
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EventItemInformation" /> class.
            /// </summary>
            /// <param name="id">event id</param>
            /// <param name="level">log level</param>
            /// <param name="messageTemplate">message template</param>
            internal EventItemInformation(int id, LogLevel level, string messageTemplate)
            {
                this.Id = id;
                this.Level = level;
                this.MessageTemplate = messageTemplate;
            }

            /// <summary>
            /// Gets event id
            /// </summary>
            internal int Id { get; private set; }

            /// <summary>
            /// Gets event log level
            /// </summary>
            internal LogLevel Level { get; private set; }

            /// <summary>
            /// Gets message template
            /// </summary>
            internal string MessageTemplate { get; private set; }
        }
    }
}
