namespace AjLanguage.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public class PrintSubroutine : ICallable
    {
        public int Arity { get { return -1; } }

        public IBindingEnvironment Environment { get { return null; } }

        public object Invoke(IBindingEnvironment environemnt, object[] arguments)
        {
            return this.Invoke(arguments);
        }

        public object Invoke(object[] arguments)
        {
            Machine machine = Machine.Current;

            foreach (object argument in arguments)
                machine.Out.Write(argument.ToString());

            return null;
        }
    }
}
