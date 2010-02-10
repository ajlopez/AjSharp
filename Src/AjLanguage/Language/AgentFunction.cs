namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;

    public class AgentFunction : ICallable
    {
        private ICallable function;

        public AgentFunction(ICallable function)
        {
            this.function = function;
        }

        public int Arity { get { return this.function.Arity; } }

        public IBindingEnvironment Environment { get { return this.function.Environment; } }

        public object Call(IObject obj, params object[] parameters)
        {
            return obj.Invoke(this, parameters);
        }

        public object Invoke(object[] arguments)
        {
            return this.Invoke(this.function.Environment, arguments);
        }

        public object Invoke(IBindingEnvironment environment, object[] arguments)
        {
            return this.function.Invoke(environment, arguments);
        }
    }
}
