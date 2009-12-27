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
        [ThreadStatic]
        private static FunctionStatus currentFunctionStatus;

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
            // TODO use only one environment
            this.environment = new LocalBindingEnvironment(this.environment);
            if (iscurrent)
                this.SetCurrent();
        }

        public static FunctionStatus CurrentFunctionStatus
        {
            get
            {
                if (currentFunctionStatus == null)
                    currentFunctionStatus = new FunctionStatus();
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

        public void SetCurrent()
        {
            current = this;
        }
    }
}
