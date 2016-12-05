//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IColumn.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace ereadian.dbdocgen.core
{
    public interface IColumn
    {
        ITable Table { get; set; }
        string Name { get; set; }
    }
}
