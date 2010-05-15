namespace AjLanguage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Hosting;

    public class Machine
    {
        [ThreadStatic]
        private static FunctionStatus currentFunctionStatus;

        [ThreadStatic]
        private static Machine current;

        private BindingEnvironment environment = new BindingEnvironment();

        private Dictionary<Guid, IHost> hosts = new Dictionary<Guid, IHost>();

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

        public IHost Host { get; set; }

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

        public void RegisterHost(IHost host)
        {
            this.hosts[host.Id] = host;
        }

        public IHost GetHost(Guid id)
        {
            return this.hosts[id];
        }

        public void SetCurrent()
        {
            current = this;
        }

        public static void SetCurrent(Machine machine)
        {
            current = machine;
        }
    }
}
