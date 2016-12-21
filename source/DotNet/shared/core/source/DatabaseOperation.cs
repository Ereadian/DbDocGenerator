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
        /// Initializes a new instance of the <see cref="DatabaseOperation" /> class.
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="configuration">database configuration</param>
        public DatabaseOperation(string connectionString, IDatabaseConfiguration configuration)
        {
            this.ConnectionString = connectionString;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets database connection string
        /// </summary>
        protected string ConnectionString { get; private set; }

        /// <summary>
        /// Gets database configuration
        /// </summary>
        protected IDatabaseConfiguration Configuration { get; private set; }

        /// <summary>
        /// Create connections instance
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
        /// Analyze database 
        /// </summary>
        /// <returns>database schema</returns>
        public abstract IDatabaseAnalysisResult Analyze();

        /// <summary>
        /// Execute reader
        /// </summary>
        /// <param name="commandName">command name</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>data reader</returns>
        public virtual T ExecuteReader<T>(
            string commandName, 
            Func<IDataReader, T> readData, 
            IReadOnlyDictionary<string, object> parameters = null)
        {
            return this.Execute(
                commandName, 
                command =>
                {
                    T data;
                    using (var reader = command.ExecuteReader())
                    {
                        data = readData(reader);
                    }

                    return data;
                },
                parameters);
        }

        /// <summary>
        /// Execute scalar
        /// </summary>
        /// <param name="commandName">command name</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>scalar value</returns>
        public object ExecuteScalar(string commandName, IReadOnlyDictionary<string, object> parameters = null)
        {
            object value = this.Execute(commandName, command => command.ExecuteScalar(), parameters);
            return value == DBNull.Value ? null : value;
        }

        /// <summary>
        /// execute command
        /// </summary>
        /// <typeparam name="T">type of return value</typeparam>
        /// <param name="commandName">command name</param>
        /// <param name="func">function for command</param>
        /// <param name="parameters">command parameters</param>
        /// <returns>return value</returns>
        public T Execute<T>(string commandName, Func<IDbCommand, T> func, IReadOnlyDictionary<string, object> parameters = null)
        {
            T value;

            using (var connection = this.CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = this.Configuration.GetCommand(commandName);
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
