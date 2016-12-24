//------------------------------------------------------------------------------------------------------------------------------------------ 
// <copyright file="RazorTemplateBase.cs" company="Ereadian"> 
//     Copyright (c) Ereadian.  All rights reserved. 
// </copyright> 
//------------------------------------------------------------------------------------------------------------------------------------------ 

namespace Ereadian.DatabaseDocumentGenerator.Core.Formater
{
    using System;
    using System.Text;

    public abstract class RazorTemplateBase<TModel>
    {
        public TModel Model { get; set; }

        public StringBuilder Buffer { get; set; }

        protected RazorTemplateBase()
        {
            this.Buffer = new StringBuilder();
        }

        public abstract void Execute();

        public virtual void Write(object value)
        {
            WriteLiteral(value);
        }

        public virtual void WriteLiteral(object value)
        {
            Buffer.Append(value);
        }

        /// <summary>
        /// This method is used to write out attribute values using
        /// some funky nested tuple storage.
        /// 
        /// Handles situations like href="@Model.Entry.Id"
        /// 
        /// This call comes in from the Razor runtime parser
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="tokens"></param>
        public virtual void WriteAttribute(string attr,
                                           //params object[] parms)
                                           Tuple<string, int> token1,
                                           Tuple<string, int> token2,
                                           Tuple<Tuple<string, int>,
                                           Tuple<object, int>, bool> token3)

        {
            object value = null;

            if (token3 != null)
                value = token3.Item2.Item1;
            else
                value = string.Empty;

            var output = token1.Item1 + value.ToString() + token2.Item1;

            WriteLiteral(output);
        }

        /// <summary>
        /// This method is used to write out attribute values using
        /// some funky nested tuple storage.
        /// 
        /// Handles situations like href="@Model.Entry.Id"
        /// 
        /// This call comes in from the Razor runtime parser
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="tokens"></param>
        public virtual void WriteAttribute(string attr,
                                   //params object[] parms)
                                   Tuple<string, int> token1,
                                   Tuple<string, int> token2,
                                   Tuple<Tuple<string, int>,Tuple<string, int>, bool> token3,
                                   Tuple<Tuple<string, int>,Tuple<object, int>, bool> token4)
        {
            //            WriteAttribute("href", 
            //                Tuple.Create(" href=\"", 395), 
            //                Tuple.Create("\"", 452), 
            //                Tuple.Create(Tuple.Create("", 402), Tuple.Create<System.Object, System.Int32>("Value", 402), false),
            //                Tuple.Create(Tuple.Create("", 439), Tuple.Create("?action=login", 439), true)            
            object value = null;
            object textval = null;
            if (token3 != null)
                value = token3.Item2.Item1;
            else
                value = string.Empty;

            if (token4 != null)
                textval = token4.Item2.Item1.ToString();
            else
                textval = string.Empty;

            var output = token1.Item1 + value.ToString() + textval.ToString() + token2.Item1;

            WriteLiteral(output);
        }
    }
}
