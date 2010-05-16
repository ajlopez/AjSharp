using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AjLanguage.Commands;
using AjLanguage.Expressions;
using AjLanguage.Language;

namespace AjLanguage.Hosting.Remoting
{
    public class RemotingHostClient : IHost
    {
        private IHost host;

        public RemotingHostClient(string address)
        {
            this.host = (IHost)Activator.GetObject(typeof(IHost), address);
        }

        public Guid Id
        {
            get { return this.host.Id; }
        }

        public void Execute(ICommand command)
        {
            this.host.Execute(command);
        }

        public object Evaluate(IExpression expression)
        {
            return this.host.Evaluate(expression);
        }

        public object Invoke(IObject receiver, string name, params object[] arguments)
        {
            return this.host.Invoke(receiver, name, arguments);
        }

        public object Invoke(Guid receiver, string name, params object[] arguments)
        {
            return this.host.Invoke(receiver, name, arguments);
        }

        public object ResultToObject(object result)
        {
            throw new NotImplementedException();
        }
    }
}
