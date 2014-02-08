namespace AjSharp.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Host : AjLanguage.Hosting.Host
    {
        public Host()
            : this(new AjSharpMachine())
        {
        }

        public Host(AjSharpMachine machine)
            : base(machine)
        {
        }
    }
}
