//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="Program.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.UI.ConsoleApplication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Configuration;
    using Core;
    using Core.SqlServer;

    class Program
    {
        static void Main(string[] args)
        {
            var providerName = ConfigurationManager.AppSettings["provider"];
            var connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            var configurationProvider = Singleton<ObjectResolverFactory>.Instance.GetResolver<IDatabaseConfigurationProvider>().Resolve();
            var configuration = configurationProvider[providerName];
            var databaseOperation = new SqlServerDatabaseOperation(connectionString, configuration);
            var result = databaseOperation.Analyze();
        }
    }
}
