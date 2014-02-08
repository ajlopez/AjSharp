namespace AjSharp.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AjLanguage;

    public class WcfHostServer : AjLanguage.Hosting.Wcf.WcfHostServer
    {
        public WcfHostServer(string address)
            : this(new AjSharpMachine(false), address)
        {
        }

        public WcfHostServer(Machine machine, string address)
            : base(machine, address)
        {
        }
    }
}
