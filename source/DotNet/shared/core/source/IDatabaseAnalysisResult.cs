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
        /// Gets or sets tables
        /// </summary>
        IReadOnlyDictionary<string, ITable> Tables { get; set; }
    }
}
