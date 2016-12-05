//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="IColumn.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace ereadian.dbdocgen.core
{
    public interface IDataType
    {
        string TypeName { get; set; }
        bool RequireSize { get; set; }
    }
}
