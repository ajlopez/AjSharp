﻿namespace AjSharp.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class EndOfInputException : Exception
    {
        public EndOfInputException()
            : this("End of Input")
        {
        }

        public EndOfInputException(string msg)
            : base(msg)
        {
        }
    }
}
