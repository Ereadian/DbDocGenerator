//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="CollectionExtensions.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Data.SqlClient;
    using System.Globalization;

    public class SqlServerDatabaseOperation : DatabaseOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseOperation" /> class.
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="configuration">database configuration</param>
        public SqlServerDatabaseOperation(string connectionString, IDatabaseConfiguration configuration) 
            : base(connectionString, configuration)
        {
        }

        /// <summary>
        /// Gets resolver factory
        /// </summary>
        protected virtual IObjectResolverFactory ResolverFactory
        {
            get
            {
                return Singleton<ObjectResolverFactory>.Instance;
            }
        }

        #region Implement base class
        /// <summary>
        /// Add parameters to command
        /// </summary>
        /// <param name="command">database command</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">parameter value</param>
        public override void AddParameter(IDbCommand command, string parameterName, object parameterValue)
        {
            var parameter = new SqlParameter(parameterName, parameterValue ?? DBNull.Value);
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Create connections instance
        /// </summary>
        /// <returns>database connection</returns>
        public override IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(this.ConnectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Analyze database 
        /// </summary>
        /// <returns>database schema</returns>
        public override IDatabaseAnalysisResult Analyze()
        {
            var result = this.ResolverFactory.GetResolver<IDatabaseAnalysisResult>().Resolve();
            result.Tables = this.GetTables();
            return result;
        }
        #endregion Implement base class

        protected IReadOnlyDictionary<string, ITable> GetTables()
        {
            return this.ExecuteReader(
                "GetTables",
                reader =>
                {
                    var tables = new Dictionary<string, ITable>(StringComparer.OrdinalIgnoreCase);
                    var tableResolver = this.ResolverFactory.GetResolver<ITable>();
                    while (reader.Read())
                    {
                        var table = tableResolver.Resolve();
                        table.SchemaName = reader["TABLE_SCHEMA"] as string;
                        table.TableName = reader["TABLE_NAME"] as string;
                        table.DisplayName = string.Format(CultureInfo.InvariantCulture, "[{0}].[{1}]", table.SchemaName, table.TableName);
                        tables.Add(table.DisplayName, table);
                    }

                    return tables;
                });
        }
    }
}
