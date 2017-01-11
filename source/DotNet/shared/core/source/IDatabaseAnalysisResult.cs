//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IDatabaseAnalysisResult.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Database Analysis result interface
    /// </summary>
    public interface IDatabaseAnalysisResult
    {
        /// <summary>
        /// Gets or sets title
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets tables
        /// </summary>
        IReadOnlyDictionary<string, ITable> Tables { get; set; }

        /// <summary>
        /// Gets or sets views
        /// </summary>
        IReadOnlyDictionary<string, IView> Views { get; set; }

        /// <summary>
        /// Gets or sets stored procedures
        /// </summary>
        IReadOnlyDictionary<string, IRoutine> StoredProcedures { get; set; }

        /// <summary>
        /// Gets or sets functions
        /// </summary>
        IReadOnlyDictionary<string, IRoutine> Functions { get; set; }
    }
}
