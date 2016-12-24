//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="SqlServerDatabaseOperation.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// SQL server database operation
    /// </summary>
    public class SqlServerDatabaseOperation : DatabaseOperation
    {
        /// <summary>
        /// Table short type name in sys.objects
        /// </summary>
        private const string TableTypeShortName = "U";

        /// <summary>
        /// Stored procedure short type name in sys.objects
        /// </summary>
        private const string ProcedureTypeShortName = "P";

        /// <summary>
        /// Function short type name in sys.objects
        /// </summary>
        private const string FunctionTypeShortName = "FN";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDatabaseOperation" /> class.
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
            this.UpdateIndexes(result.Tables);
            var routines = this.GetRoutines(result);
            this.UpdateRoutineParameters(routines);
            this.UpdateReferences(result, routines);
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

                        constraint.Table = primaryKeyTable;
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
        /// Update table index information
        /// </summary>
        /// <param name="tables">table collection</param>
        protected void UpdateIndexes(IReadOnlyDictionary<string, ITable> tables)
        {
            this.ExecuteReader<object>(
                "GetIndexes",
                reader =>
                {
                    ITable table = null;
                    List<IIndex> indexes = null;
                    IIndex index = null;
                    List<IColumn> columns = null;
                    var indexResolver = this.ResolverFactory.GetResolver<IIndex>();
                    while (reader.Read())
                    {
                        var tableSchema = GetData<string>(reader, "SchemaName");
                        var tableName = GetData<string>(reader, "TableName");
                        var indexName = GetData<string>(reader, "IndexName");
                        var indexDisplayName = GetDisplayName(tableSchema, indexName);

                        if ((table == null)
                            || !table.SchemaName.Equals(tableSchema, StringComparison.OrdinalIgnoreCase)
                            || !table.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase))
                        {
                            var tableDisplayName = GetDisplayName(tableSchema, tableName);
                            if (!tables.TryGetValue(tableDisplayName, out table))
                            {
                                string errorMessage = string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Could not find foreign key table \"{0}\" for index \"{1}\".",
                                    tableDisplayName,
                                    indexDisplayName);
                                throw new ApplicationException(errorMessage);
                            }

                            indexes = new List<IIndex>();
                            table.Indexes = indexes;
                        }

                        if ((index == null)
                            || (!index.IndexSchema.Equals(tableSchema, StringComparison.OrdinalIgnoreCase))
                            || (!index.IndexName.Equals(indexName, StringComparison.OrdinalIgnoreCase)))
                        {
                            index = indexResolver.Resolve();
                            indexes.Add(index);
                            index.Table = table;
                            index.IndexDisplayName = GetDisplayName(tableSchema, indexName);
                            index.IndexSchema = tableSchema;
                            index.IndexName = indexName;
                            columns = new List<IColumn>();
                            index.Columns = columns;
                        }

                        var columnName = GetData<string>(reader, "ColumnName");
                        IColumn column = table.Columns.FirstOrDefault(c => c.Name == columnName);
                        if (column == null)
                        {
                            var errorMessage = string.Format(
                                CultureInfo.InvariantCulture,
                                "Could not find indexed column \"{0}\" in table \"{1}\".",
                                columnName,
                                table.DisplayName);
                            throw new ApplicationException(errorMessage);
                        }

                        index.IsPrimary = GetData<bool>(reader, "IsPrimaryKey");
                        index.IsUniqueIndex = GetData<bool>(reader, "IsUniqueIndex");
                        columns.Add(column);
                    }

                    return null;
                });
        }

        /// <summary>
        /// Get routines like stored procedures and functions
        /// </summary>
        /// <param name="result">data analysis result</param>
        /// <returns>routine dictionary</returns>
        protected IReadOnlyDictionary<string, IRoutine> GetRoutines(IDatabaseAnalysisResult result)
        {
            var routineCollection = this.ExecuteReader(
                "GetRoutines",
                reader =>
                {
                    var routines = new Dictionary<string, IRoutine>(StringComparer.OrdinalIgnoreCase);
                    var resolver = this.ResolverFactory.GetResolver<IRoutine>();
                    while (reader.Read())
                    {
                        var routine = resolver.Resolve();
                        routine.SchemaName = GetData<string>(reader, "ROUTINE_SCHEMA");
                        routine.RoutineName = GetData<string>(reader, "ROUTINE_NAME");
                        routine.RoutineType = GetData<string>(reader, "ROUTINE_TYPE");
                        routine.DataTypeName = GetData<string>(reader, "DATA_TYPE");
                        routine.StringSize = GetData<int?>(reader, "CHARACTER_MAXIMUM_LENGTH");
                        routine.NumericPrecision = GetData<int?>(reader, "NUMERIC_PRECISION");
                        routine.NumericScale = GetData<int?>(reader, "NUMERIC_SCALE");
                        routine.SoruceCode = GetData<string>(reader, "ROUTINE_DEFINITION");
                        routine.DisplayName = GetDisplayName(routine.SchemaName, routine.RoutineName);
                        routines.Add(routine.DisplayName, routine);
                    }

                    return routines;
                });

            var collection = new Dictionary<string, Dictionary<string, IRoutine>>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, IRoutine> currentRoutines;
            foreach (var pair in routineCollection)
            {
                var typeName = pair.Value.RoutineType;
                if (!collection.TryGetValue(typeName, out currentRoutines))
                {
                    currentRoutines = new Dictionary<string, IRoutine>(StringComparer.OrdinalIgnoreCase);
                    collection.Add(typeName, currentRoutines);
                }

                currentRoutines.Add(pair.Key, pair.Value);
            }

            if (collection.TryGetValue("PROCEDURE", out currentRoutines))
            {
                result.StoredProcedures = currentRoutines;
            }

            if (collection.TryGetValue("FUNCTION", out currentRoutines))
            {
                result.Functions = currentRoutines;
            }

            return routineCollection;
        }

        /// <summary>
        /// Update routine parameters
        /// </summary>
        /// <param name="routines">routine collection</param>
        protected void UpdateRoutineParameters(IReadOnlyDictionary<string, IRoutine> routines)
        {
            this.ExecuteReader<object>(
                "GetRoutineParameters",
                reader =>
                {
                    IRoutine routine = null;
                    List<IParameter> parameters = null;
                    var resolver = this.ResolverFactory.GetResolver<IParameter>();
                    while (reader.Read())
                    {
                        var routineSchema = GetData<string>(reader, "ROUTINE_SCHEMA");
                        var routineName = GetData<string>(reader, "ROUTINE_NAME");

                        if ((routine == null)
                            || !routine.SchemaName.Equals(routineSchema, StringComparison.OrdinalIgnoreCase)
                            || !routine.RoutineName.Equals(routineName, StringComparison.OrdinalIgnoreCase))
                        {
                            var routineDisplayName = GetDisplayName(routineSchema, routineName);
                            if (!routines.TryGetValue(routineDisplayName, out routine))
                            {
                                var errorMessage = string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Could not found routine \"{0}\" while process parameters",
                                    routineDisplayName);
                                throw new ApplicationException(errorMessage);
                            }

                            parameters = new List<IParameter>();
                            routine.Parameters = parameters;
                        }

                        var parameter = resolver.Resolve();
                        parameter.ParameterName = GetData<string>(reader, "PARAMETER_NAME");
                        parameter.Mode = GetData<string>(reader, "PARAMETER_MODE");
                        parameter.DataTypeName = GetData<string>(reader, "DATA_TYPE");
                        parameter.StringSize = GetData<int?>(reader, "CHARACTER_MAXIMUM_LENGTH");
                        parameter.NumericPrecision = GetData<int?>(reader, "NUMERIC_PRECISION");
                        parameter.NumericScale = GetData<int?>(reader, "NUMERIC_SCALE");
                        parameters.Add(parameter);
                    }

                    return null;
                });
        }

        /// <summary>
        /// Update references
        /// </summary>
        /// <param name="result">database analysis result</param>
        /// <param name="routines">routine collection</param>
        protected void UpdateReferences(IDatabaseAnalysisResult result, IReadOnlyDictionary<string, IRoutine> routines)
        {
            var resolver = this.ResolverFactory.GetResolver<IReference>();

            // process foreign key references
            foreach (var table in result.Tables.Values)
            {
                if (!table.ForeignKeys.IsReadOnlyNullOrEmpty())
                {
                    foreach (var foreignKey in table.ForeignKeys)
                    {
                        var referencedTable = foreignKey.Table;
                        if (referencedTable.References == null)
                        {
                            referencedTable.References = new SortedList<string, IReference>();
                        }

                        var reference = resolver.Resolve();
                        reference.EntityDisplayName = table.DisplayName;
                        reference.EntitySchema = table.SchemaName;
                        reference.EntityName = table.TableName;
                        reference.EntityType = TableTypeShortName;
                        var referenceKey = GetReferenceKey(reference);
                        referencedTable.References[referenceKey] = reference;
                    }
                }
            }

            // process cross entity references
            this.ExecuteReader<object>(
                "GetReferences",
                reader =>
                {
                    while (reader.Read())
                    {
                        var referenceShema = GetData<string>(reader, "REFERENCE_ENTITY_SCHEMA");
                        var referenceName = GetData<string>(reader, "REFERENCE_ENTITY_NAME");
                        var referenceType = GetData<string>(reader, "REFERENCE_ENTITY_TYPE");
                        var referenceDisplayName = GetDisplayName(referenceShema, referenceName);

                        var reference = resolver.Resolve();
                        reference.EntityDisplayName = GetDisplayName(referenceShema, referenceName);
                        reference.EntitySchema = GetData<string>(reader, "REFERENCE_ENTITY_SCHEMA");
                        reference.EntityName = GetData<string>(reader, "REFERENCE_ENTITY_NAME");
                        reference.EntityType = GetData<string>(reader, "REFERENCE_ENTITY_TYPE");
                        var referenceKey = GetReferenceKey(reference);

                        switch (referenceType.ToUpperInvariant())
                        {
                            case TableTypeShortName:
                                ITable table;
                                if (result.Tables.IsReadOnlyNullOrEmpty() || !result.Tables.TryGetValue(referenceDisplayName, out table))
                                {
                                    var errorMessage = string.Format(
                                        CultureInfo.InvariantCulture,
                                        "Could not find table \"{0}\" while building reference for \"{1}\"",
                                        referenceDisplayName,
                                        reference.EntityDisplayName);
                                    throw new ApplicationException(errorMessage);
                                }

                                if (table.References == null)
                                {
                                    table.References = new SortedList<string, IReference>();
                                }

                                table.References[referenceKey] = reference;
                                break;
                            case ProcedureTypeShortName:
                            case FunctionTypeShortName:
                                IRoutine routine;
                                if (routines.IsReadOnlyNullOrEmpty() || !routines.TryGetValue(referenceDisplayName, out routine))
                                {
                                    var errorMessage = string.Format(
                                        CultureInfo.InvariantCulture,
                                        "Could not find routine \"{0}\" while building reference for \"{1}\"",
                                        referenceDisplayName,
                                        reference.EntityDisplayName);
                                    throw new ApplicationException(errorMessage);
                                }

                                if (routine.References == null)
                                {
                                    routine.References = new SortedList<string, IReference>();
                                }

                                routine.References[referenceKey] = reference;
                                break;
                        }
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

        /// <summary>
        /// Get reference key
        /// </summary>
        /// <param name="reference">reference instance</param>
        /// <returns>reference key</returns>
        private static string GetReferenceKey(IReference reference)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}:{1}:{2}",
                reference.EntityType,
                reference.EntitySchema,
                reference.EntityName).ToUpperInvariant();
        }
    }
}
