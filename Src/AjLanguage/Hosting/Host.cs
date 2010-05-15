namespace AjLanguage.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Expressions;

    public class Host : IHost
    {
        private Guid id = Guid.NewGuid();
        private Machine machine;
        private Dictionary<object, Guid> objectids = new Dictionary<object, Guid>();
        private Dictionary<Guid, object> objects = new Dictionary<Guid, object>();

        public Host()
        {
            this.machine = new Machine(false);
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

        public Guid Evaluate(IExpression expression)
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

            return this.ObjectoToGuid(result);
        }

        public Guid Invoke(Guid objid, string name, params object[] arguments)
        {
            object receiver = this.objectids[objid];

            object result = ObjectUtilities.GetValue(receiver, name, arguments);

            return this.ObjectoToGuid(result);
        }

        public object GetObject(Guid objid)
        {
            return this.objects[objid];
        }

        private Guid ObjectoToGuid(object result)
        {
            if (result == null)
                return Guid.Empty;

            if (this.objectids.ContainsKey(result))
                return this.objectids[result];

            Guid guid = Guid.NewGuid();

            this.objectids[result] = guid;
            this.objects[guid] = result;

            return guid;
        }
    }
}

