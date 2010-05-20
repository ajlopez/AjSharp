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

            if (Machine.Current != null)
                Machine.Current.RegisterHost(this);
        }

        public RemotingHostClient(string hostname, int port, string name)
            : this(MakeAddress(hostname, port, name))
        {
        }

        public Guid Id
        {
            get { return this.host.Id; }
        }

        public bool IsLocal { get { return false; } }

        public void RegisterHost(string address)
        {
            this.host.RegisterHost(address);
        }

        public void Execute(ICommand command)
        {
            this.host.Execute(command);
        }

        public object Evaluate(IExpression expression)
        {
            return this.ClientMarshalling(this.host.Evaluate(expression));
        }

        public object Invoke(ICallable function, params object[] arguments)
        {
            return this.ClientMarshalling(this.host.Invoke(function, arguments));
        }

        public object Invoke(IObject receiver, string name, params object[] arguments)
        {
            return this.ClientMarshalling(this.host.Invoke(receiver, name, arguments));
        }

        public object Invoke(IObject receiver, ICallable method, params object[] arguments)
        {
            return this.ClientMarshalling(this.host.Invoke(receiver, method, arguments));
        }

        public object Invoke(Guid receiver, string name, params object[] arguments)
        {
            return this.ClientMarshalling(this.host.Invoke(receiver, name, arguments));
        }

        public object Invoke(Guid receiver, ICallable method, params object[] arguments)
        {
            return this.ClientMarshalling(this.host.Invoke(receiver, method, arguments));
        }

        public object ResultToObject(object result)
        {
            throw new NotImplementedException();
        }

        private object ClientMarshalling(object result)
        {
            if (result == null)
                return null;

            if (!(result is IObject))
                return result;

            if (result is ObjectProxy)
                return result;

            if (!System.Runtime.Remoting.RemotingServices.IsTransparentProxy(result))
                return result;

            return new ClientObject(this.host, (IObject)result);
        }

        private static string MakeAddress(string hostname, int port, string name)
        {
            return string.Format("tcp://{0}:{1}/{2}", hostname, port, name);
        }
    }
}
