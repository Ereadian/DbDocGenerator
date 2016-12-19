//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="DatabaseOperation.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Database operations
    /// </summary>
    public abstract class DatabaseOperation
    {
        /// <summary>
        /// database connection string
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// database configuration
        /// </summary>
        private readonly IDatabaseConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseOperation" /> class.
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="configuration">database configuration</param>
        public DatabaseOperation(string connectionString, IDatabaseConfiguration configuration)
        {
            this.connectionString = connectionString;
            this.configuration = configuration;
        }

        /// <summary>
        /// Create connections string
        /// </summary>
        /// <returns>database connection</returns>
        public abstract IDbConnection CreateConnection();

        /// <summary>
        /// Add parameters to command
        /// </summary>
        /// <param name="command">database command</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">parameter value</param>
        public abstract void AddParameter(IDbCommand command, string parameterName, object parameterValue);

        /// <summary>
        /// Execute reader
        /// </summary>
        /// <param name="commandName">command name</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>data reader</returns>
        public virtual IDataReader ExecuteReader(string commandName, IReadOnlyDictionary<string, object> parameters = null)
        {
            return this.Execute(commandName, parameters, command => command.ExecuteReader());
        }

        /// <summary>
        /// Execute scalar
        /// </summary>
        /// <param name="commandName">command name</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>scalar value</returns>
        public object ExecuteScalar(string commandName, IReadOnlyDictionary<string, object> parameters = null)
        {
            object value = this.Execute(commandName, parameters, command => command.ExecuteScalar());
            return value == DBNull.Value ? null : value;
        }

        /// <summary>
        /// execute command
        /// </summary>
        /// <typeparam name="T">type of return value</typeparam>
        /// <param name="commandName">command name</param>
        /// <param name="parameters">command parameters</param>
        /// <param name="func">function for command</param>
        /// <returns>return value</returns>
        protected T Execute<T>(string commandName, IReadOnlyDictionary<string, object> parameters, Func<IDbCommand, T> func)
        {
            T value;

            using (var connection = this.CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = this.configuration.GetCommand(commandName);
                    command.CommandType = CommandType.Text;

                    if (!parameters.IsReadOnlyNullOrEmpty())
                    {
                        foreach (var pair in parameters)
                        {
                            this.AddParameter(command, pair.Key, pair.Value);
                        }
                    }

                    value = func(command);
                }
            }

            return value;
        }
    }
}
