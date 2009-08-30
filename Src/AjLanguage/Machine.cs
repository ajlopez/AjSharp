namespace AjLanguage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;

    public class Machine
    {
        private static FunctionStatus currentFunctionStatus = new FunctionStatus();
        [ThreadStatic]
        private static Machine current;

        private BindingEnvironment environment = new BindingEnvironment();

        private TextReader inreader = System.Console.In;
        private TextWriter outwriter = System.Console.Out;
        private TextWriter errwriter = System.Console.Error;

        public Machine()
            : this(true)
        {
        }

        public Machine(bool iscurrent)
        {
            if (iscurrent)
                current = this;
        }

        public static FunctionStatus CurrentFunctionStatus
        {
            get
            {
                return currentFunctionStatus;
            }

            set
            {
                currentFunctionStatus = value;
            }
        }

        public static Machine Current { get { return current; } }

        public BindingEnvironment Environment { get { return this.environment; } }

        public TextReader In
        {
            get
            {
                return this.inreader;
            }

            set
            {
                this.inreader = value;
            }
        }

        public TextWriter Out
        {
            get
            {
                return this.outwriter;
            }

            set
            {
                this.outwriter = value;
            }
        }
    }
}
