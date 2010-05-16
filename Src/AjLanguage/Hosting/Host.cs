namespace AjLanguage.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public class Host : MarshalByRefObject, IHost
    {
        private Guid id = Guid.NewGuid();
        private Machine machine;
        private Dictionary<object, Guid> objectids = new Dictionary<object, Guid>();
        private Dictionary<Guid, ObjectProxy> proxies = new Dictionary<Guid, ObjectProxy>();

        public Host()
        {
            this.machine = new Machine(false);
            this.machine.Host = this;
        }

        public Host(Machine machine)
        {
            this.machine = machine;
            this.machine.Host = this;
        }

        public Machine Machine { get { return this.machine; } }

        public Guid Id { get { return this.id; } }

        public void Execute(ICommand command)
        {
            Machine current = Machine.Current;
            try 
            {
                this.machine.SetCurrent();
                command.Execute(this.machine.Environment);            
            }
            finally 
            {
                Machine.SetCurrent(current);
            }
        }

        public object Evaluate(IExpression expression)
        {
            Machine current = Machine.Current;
            
            object result = null;
            
            try
            {
                this.machine.SetCurrent();
                result = expression.Evaluate(this.machine.Environment);
            }
            finally
            {
                Machine.SetCurrent(current);
            }

            return result;
            //return this.ResultToObject(result);
        }

        public object Invoke(Guid objid, string name, params object[] arguments)
        {
            object receiver = this.objectids[objid];

            object result = ObjectUtilities.GetValue(receiver, name, arguments);

            return this.ResultToObject(result);
        }

        public object Invoke(IObject obj, string name, params object[] arguments)
        {
            Machine current = Machine.Current;

            try
            {
                this.machine.SetCurrent();

                if (obj is ObjectProxy)
                {
                    ObjectProxy proxy = (ObjectProxy)obj;

                    if (proxy.HostId != this.Id)
                        throw new NotSupportedException();

                    return this.Invoke(proxy.ObjectId, name, arguments);
                }

                return obj.Invoke(name, arguments);
            }
            finally
            {
                Machine.SetCurrent(current);
            }
        }

        public object GetObject(Guid objid)
        {
            return this.proxies[objid].Object;
        }

        private object ResultToObject(object result)
        {
            if (result == null)
                return Guid.Empty;

            if (!(result is IObject))
                return result;

            if (this.objectids.ContainsKey(result))
                return this.objectids[result];

            ObjectProxy proxy = new ObjectProxy(result, this);

            this.objectids[result] = proxy.ObjectId;
            this.proxies[proxy.ObjectId] = proxy;

            return proxy;
        }
    }
}

