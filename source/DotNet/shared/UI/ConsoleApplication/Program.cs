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
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            var title = ConfigurationManager.AppSettings["title"];
            var providerName = ConfigurationManager.AppSettings["provider"];
            var sampleCount = int.Parse(ConfigurationManager.AppSettings["samples"]);
            var connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            var resolverFactory = Singleton<ObjectResolverFactory>.Instance;

            var configurationProvider = resolverFactory.GetResolver<IDatabaseConfigurationProvider>().Resolve();
            var configuration = configurationProvider[providerName];
            var databaseOperation = new SqlServerDatabaseOperation(connectionString, configuration, sampleCount);
            var result = databaseOperation.Analyze();

            var formatter = resolverFactory.GetResolver<IFormatter>().Resolve();
            string filename = args.Length > 0 ? args[0] : "database.html";
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                formatter.Format(title, result, stream);
            }
        }
    }
}
