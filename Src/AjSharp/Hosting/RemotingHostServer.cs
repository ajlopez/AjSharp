using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AjLanguage;

namespace AjSharp.Hosting
{
    public class RemotingHostServer : AjLanguage.Hosting.Remoting.RemotingHostServer
    {
        public RemotingHostServer(int port, string name)
            : this(new AjSharpMachine(false), port, name)
        {
        }

        public RemotingHostServer(Machine machine, int port, string name)
            : base(machine, port, name)
        {
        }
    }
}
