using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AjLanguage;
using AjSharp.Compiler;
using AjLanguage.Commands;

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

        public override void Execute(string commandtext)
        {
            Machine current = Machine.Current;

            try
            {
                Machine.SetCurrent(this.Machine);
                Parser parser = new Parser(commandtext);

                ICommand command;

                while ((command = parser.ParseCommand()) != null)
                    command.Execute(this.Machine.Environment);
            }
            finally
            {
                Machine.SetCurrent(current);
            }
        }
    }
}
