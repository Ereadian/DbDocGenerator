//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IBaseContainer.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Table/view base interface
    /// </summary>
    public interface IBaseContainer
    {
        /// <summary>
        /// Gets or sets table/view display name
        /// </summary>
        /// <example>[dbo].[Users]</example>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets Schema Name (for example, "DBO")
        /// </summary>
        string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets Table Name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets columns
        /// </summary>
        IReadOnlyList<IColumn> Columns { get; set; }

        /// <summary>
        /// referenced by entities
        /// </summary>
        /// <remarks>
        /// key is [type][schema][name]. for example, fn.dbo.fnGetUser.
        /// </remarks>
        SortedList<string, IReference> References { get; set; }
    }
}
