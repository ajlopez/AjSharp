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

        public object Invoke(ICallable function, params object[] arguments)
        {
            Machine current = Machine.Current;
            try
            {
                this.machine.SetCurrent();
                return function.Invoke(arguments);
            }
            finally
            {
                Machine.SetCurrent(current);
            }
        }

        public object Evaluate(IExpression expression)
        {
            if (expression == null)
                return null;

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
            object receiver = this.GetObject(objid);

            object result = ObjectUtilities.GetValue(receiver, name, arguments);

            return result;
            //return this.ResultToObject(result);
        }

        public object Invoke(Guid objid, ICallable method, params object[] arguments)
        {
            IObject receiver = (IObject) this.GetObject(objid);

            return receiver.Invoke(method, arguments);
            //return this.ResultToObject(receiver.Invoke(method, arguments));
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

        public object Invoke(IObject obj, ICallable method, params object[] arguments)
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

                    return this.Invoke(proxy.ObjectId, method, arguments);
                }

                return obj.Invoke(method, arguments);
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

        public object ResultToObject(object result)
        {
            if (result == null)
                return null;

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

