//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IView.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    /// <summary>
    /// View column usage
    /// </summary>
    public interface IColumnUsage
    {
        /// <summary>
        /// View column
        /// </summary>
        IColumn ViewColumn { get; set; }

        /// <summary>
        /// Container
        /// </summary>
        IBaseContainer Container { get; set; }
    }
}
