using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AjSharp.Hosting
{
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
