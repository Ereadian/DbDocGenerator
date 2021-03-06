﻿//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="ITable.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Table definition interface
    /// </summary>
    public interface ITable : IBaseContainer
    {
        /// <summary>
        /// Gets or sets primary key
        /// </summary>
        IConstraint PrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets table indexes
        /// </summary>
        IReadOnlyList<IIndex> Indexes { get; set; }

        /// <summary>
        /// Gets or sets foreign keys
        /// </summary>
        IReadOnlyList<IConstraint> ForeignKeys { get; set; }
    }
}
