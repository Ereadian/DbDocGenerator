//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IView.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// View interface
    /// </summary>
    public interface IView : IBaseContainer
    {
        /// <summary>
        /// Gets or sets view column usages
        /// </summary>
        IReadOnlyList<IColumnUsage> ColumnUsages { get; set; }
    }
}
