namespace AjSharp.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public abstract class LexerException : Exception
    {
        protected LexerException(string msg)
            : base(msg)
        {
        }
    }
}
