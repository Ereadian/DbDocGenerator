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
            this.UpdateColumns(result.Tables);
            this.UpdateTablePrimaryKeys(result.Tables);
            this.UpdateTableForeignKeys(result.Tables);
            return result;
        }
        #endregion Implement base class

        /// <summary>
        /// Get tables
        /// </summary>
        /// <returns>table collection</returns>
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
                        table.DisplayName = GetDisplayName(table.SchemaName, table.TableName);
                        tables.Add(table.DisplayName, table);
                    }

                    return tables;
                });
        }

        /// <summary>
        /// Update table by add columns
        /// </summary>
        /// <param name="tables">table collection</param>
        protected void UpdateColumns(IReadOnlyDictionary<string, ITable> tables)
        {
            this.ExecuteReader<object>(
                "GetColumns",
                reader =>
                {
                    ITable table = null;
                    List<IColumn> columns = null;
                    var columnResolver = this.ResolverFactory.GetResolver<IColumn>();
                    while (reader.Read())
                    {
                        var tableSchema = GetData<string>(reader, "TABLE_SCHEMA");
                        var tableName = GetData<string>(reader, "TABLE_NAME");
                        var columnName = GetData<string>(reader, "COLUMN_NAME"); ;

                        if ((table == null) 
                            || !table.SchemaName.Equals(tableSchema, StringComparison.OrdinalIgnoreCase) 
                            || !table.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase))
                        {
                            var tableDisplayName = GetDisplayName(tableSchema, tableName);
                            if (!tables.TryGetValue(tableDisplayName, out table))
                            {
                                string errorMessage = string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Could not find table \"{0}\" for column \"{1}\".",
                                    tableDisplayName,
                                    columnName);
                                throw new ApplicationException(errorMessage);
                            }

                            columns = new List<IColumn>();
                            table.Columns = columns;
                        }

                        var column = columnResolver.Resolve();
                        column.Table = table;
                        column.Name = columnName;
                        column.DataTypeName = GetData<string>(reader, "DATA_TYPE");
                        column.StringSize = GetData<int?>(reader, "CHARACTER_MAXIMUM_LENGTH");
                        column.NumericPrecision = GetData<int?>(reader, "NUMERIC_PRECISION");
                        column.NumericScale = GetData<int?>(reader, "NUMERIC_SCALE");
                        column.IsNullable = GetData<string>(reader, "IS_NULLABLE").Equals("YES", StringComparison.OrdinalIgnoreCase);
                        column.DefaultValue = GetData<string>(reader, "COLUMN_DEFAULT");
                        columns.Add(column);
                    }
                    return null;
                });
        }

        /// <summary>
        /// Update table primary key
        /// </summary>
        /// <param name="tables">table collection</param>
        protected void UpdateTablePrimaryKeys(IReadOnlyDictionary<string, ITable> tables)
        {
            this.ExecuteReader<object>(
                "GetPrimaryKeys",
                reader =>
                {
                    ITable table = null;
                    List<KeyValuePair<IColumn, IColumn>> columns = null;
                    var constraintResolver = this.ResolverFactory.GetResolver<IConstraint>();
                    while (reader.Read())
                    {
                        var tableSchema = GetData<string>(reader, "TABLE_SCHEMA");
                        var tableName = GetData<string>(reader, "TABLE_NAME");
                        var constraintSchema = GetData<string>(reader, "CONSTRAINT_SCHEMA");
                        var constraintName = GetData<string>(reader, "CONSTRAINT_NAME");
                        var constraintDisplayName = GetDisplayName(constraintSchema, constraintName);
                        var columnName = GetData<string>(reader, "COLUMN_NAME");

                        if ((table == null)
                            || !table.SchemaName.Equals(tableSchema, StringComparison.OrdinalIgnoreCase)
                            || !table.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase))
                        {
                            var tableDisplayName = GetDisplayName(tableSchema, tableName);
                            if (!tables.TryGetValue(tableDisplayName, out table))
                            {
                                string errorMessage = string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Could not find table \"{0}\" for constraint \"{1}\".",
                                    tableDisplayName,
                                    constraintDisplayName);
                                throw new ApplicationException(errorMessage);
                            }

                            var constraint = constraintResolver.Resolve();
                            table.PrimaryKey = constraint;
                            constraint.Table = table;
                            constraint.DisplayName = constraintDisplayName;
                            constraint.SchemaName = constraintSchema;
                            constraint.ConstraintName = constraintName;
                            columns = new List<KeyValuePair<IColumn, IColumn>>();
                            constraint.Columns = columns;
                        }

                        IColumn column = table.Columns.FirstOrDefault(c => c.Name == columnName);
                        if (column == null)
                        {
                            var errorMessage = string.Format(
                                CultureInfo.InvariantCulture,
                                "Could not find column \"{0}\" in table \"{1}\" for primary key constraint \"{2}\".",
                                columnName,
                                table.DisplayName,
                                constraintDisplayName);
                            throw new ApplicationException(errorMessage);
                        }

                        columns.Add(new KeyValuePair<IColumn, IColumn>(column, null));
                    }
                    return null;
                });
        }

        /// <summary>
        /// Update table primary key
        /// </summary>
        /// <param name="tables">table collection</param>
        protected void UpdateTableForeignKeys(IReadOnlyDictionary<string, ITable> tables)
        {
            this.ExecuteReader<object>(
                "GetForeignKeys",
                reader =>
                {
                    ITable foreignKeyTable = null;
                    List<IConstraint> foreignKeys = null;
                    var constraintResolver = this.ResolverFactory.GetResolver<IConstraint>();
                    while (reader.Read())
                    {
                        var foreignKeyTableSchema = GetData<string>(reader, "FK_TABLE_SCHEMA");
                        var foreignKeyTableName = GetData<string>(reader, "FK_TABLE_NAME");
                        var constraintSchema = GetData<string>(reader, "CONSTRAINT_SCHEMA");
                        var constraintName = GetData<string>(reader, "CONSTRAINT_NAME");
                        var constraintDisplayName = GetDisplayName(constraintSchema, constraintName);

                        if ((foreignKeyTable == null)
                            || !foreignKeyTable.SchemaName.Equals(foreignKeyTableSchema, StringComparison.OrdinalIgnoreCase)
                            || !foreignKeyTable.TableName.Equals(foreignKeyTableName, StringComparison.OrdinalIgnoreCase))
                        {
                            var tableDisplayName = GetDisplayName(foreignKeyTableSchema, foreignKeyTableName);
                            if (!tables.TryGetValue(tableDisplayName, out foreignKeyTable))
                            {
                                string errorMessage = string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Could not find foreign key table \"{0}\" for constraint \"{1}\".",
                                    tableDisplayName,
                                    constraintDisplayName);
                                throw new ApplicationException(errorMessage);
                            }

                            foreignKeys = new List<IConstraint>();
                            foreignKeyTable.ForeignKeys = foreignKeys;
                        }

                        var constraint = constraintResolver.Resolve();
                        constraint.Table = foreignKeyTable;
                        constraint.DisplayName = constraintDisplayName;
                        constraint.SchemaName = constraintSchema;
                        constraint.ConstraintName = constraintName;

                        var foreignKeyColumnName = GetData<string>(reader, "FK_COLUMN_NAME");
                        IColumn foreignKeyColumn = foreignKeyTable.Columns.FirstOrDefault(c => c.Name == foreignKeyColumnName);
                        if (foreignKeyColumn == null)
                        {
                            var errorMessage = string.Format(
                                CultureInfo.InvariantCulture,
                                "Could not find foreign key column \"{0}\" in table \"{1}\" for foreign key constraint \"{2}\".",
                                foreignKeyColumnName,
                                foreignKeyTable.DisplayName,
                                constraintDisplayName);
                            throw new ApplicationException(errorMessage);
                        }

                        var primaryKeyTableSchema = GetData<string>(reader, "PK_TABLE_SCHEMA");
                        var primaryKeyTableName = GetData<string>(reader, "PK_TABLE_NAME");
                        var primaryKeyColumnName = GetData<string>(reader, "PK_COLUMN_NAME");
                        var primaryKeyTableDisplayName = GetDisplayName(primaryKeyTableSchema, primaryKeyTableName);
                        ITable primaryKeyTable;
                        if (!tables.TryGetValue(primaryKeyTableDisplayName, out primaryKeyTable))
                        {
                            string errorMessage = string.Format(
                                CultureInfo.InvariantCulture,
                                "Could not find primary key table \"{0}\" for constraint \"{1}\".",
                                primaryKeyTableDisplayName,
                                constraintDisplayName);
                            throw new ApplicationException(errorMessage);
                        }

                        IColumn primaryKeyColumn = primaryKeyTable.Columns.FirstOrDefault(c => c.Name == primaryKeyColumnName);
                        if (primaryKeyColumn == null)
                        {
                            var errorMessage = string.Format(
                                CultureInfo.InvariantCulture,
                                "Could not find primary key column \"{0}\" in table \"{1}\" for foreign key constraint \"{2}\".",
                                primaryKeyColumnName,
                                primaryKeyTable.DisplayName,
                                constraintDisplayName);
                            throw new ApplicationException(errorMessage);
                        }

                        constraint.Columns = new List<KeyValuePair<IColumn, IColumn>>()
                        {
                            new KeyValuePair<IColumn, IColumn>(foreignKeyColumn, primaryKeyColumn)
                        };

                        foreignKeys.Add(constraint);
                    }

                    return null;
                });
        }

        /// <summary>
        /// Get data from data reader
        /// </summary>
        /// <typeparam name="T">type to convert</typeparam>
        /// <param name="reader">data reader</param>
        /// <param name="columnName">column name</param>
        /// <returns>column value</returns>
        private static T GetData<T>(IDataReader reader, string columnName)
        {
            var data = reader[columnName];
            if ((data == null) || (data == DBNull.Value))
            {
                return default(T);
            }

            var type = typeof(T);
            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }

            return (T)ChangeType(data, type);
        }

        /// <summary>
        /// Change type
        /// </summary>
        /// <param name="value">value to change</param>
        /// <param name="conversionType">target type</param>
        /// <returns>data with new type (if type is different) or same value if target type is compatible</returns>
        private static object ChangeType(object value, Type conversionType)
        {
            if (conversionType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// Get display name
        /// </summary>
        /// <param name="schemaName">object schema name</param>
        /// <param name="tableName">object name</param>
        /// <returns>display name</returns>
        private static string GetDisplayName(string schemaName, string tableName)
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0}].[{1}]", schemaName, tableName);
        }
    }
}
