//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IFormatter.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.IO;

    /// <summary>
    /// Formatter interface
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Format output
        /// </summary>
        /// <param name="title">document title</param>
        /// <param name="analysisResult">analysis result</param>
        /// <param name="outputSteram">output stream</param>
        void Format(string title, IDatabaseAnalysisResult analysisResult, Stream outputSteram);
    }
}
