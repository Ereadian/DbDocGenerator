//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DatabaseConfigurationExtensions.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Database configuration extensions
    /// </summary>
    public static class DatabaseConfigurationExtensions
    {
        /// <summary>
        /// Get data type by type name
        /// </summary>
        /// <param name="configuration">database configuration instance</param>
        /// <param name="name">data type name</param>
        /// <param name="size">size of data</param>
        /// <returns>data type instance</returns>
        public static IDataType GetDataType(this IDatabaseConfiguration configuration, string name, int? size = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (string.IsNullOrEmpty(name))
            {
                const string ErrorMessage = "parameter \"name\" should not be null or blank";
                Trace.TraceError(ErrorMessage);
                throw new ArgumentException(ErrorMessage);
            }

            IDataType dataType = null;
            var supportedTypes = configuration.SupportedTypes;
            if (supportedTypes.IsReadOnlyNullOrEmpty())
            {
                return null;
            }

            if (!supportedTypes.TryGetValue(name, out dataType))
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Type \"{0}\" is not supported",
                    name);
                Trace.TraceError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            var sizeType = dataType.DataSize.SizeType;
            if ((sizeType == DataSizeType.NotRequired) || ((sizeType == DataSizeType.Maximum) && !size.HasValue))
            {
                return dataType;
            }

            if (!size.HasValue)
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Data size of type \"{0}\" is required",
                    name);
                Trace.TraceError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            return new DataType(dataType.TypeName, new DataSize(DataSizeType.Required, size));
        }

        /// <summary>
        /// Retrieve query command by name
        /// </summary>
        /// <param name="configuration">database configuration instance</param>
        /// <param name="name">query command name</param>
        /// <returns>query command</returns>
        public static string GetCommand(this IDatabaseConfiguration configuration, string name)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            var queryCommands = configuration.QueryCommands;

            string command = null;
            if (queryCommands.IsReadOnlyNullOrEmpty() || !queryCommands.TryGetValue(name, out command))
            {
                command = null;
            }

            return command;
        }
    }
}
