namespace AjLanguage.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public class PrintLineSubroutine : ICallable
    {
        public int Arity { get { return -1; } }

        public object Invoke(IBindingEnvironment environemnt, object[] arguments)
        {
            Machine machine = Machine.Current;
            foreach (object argument in arguments)
                machine.Out.Write(argument.ToString());

            machine.Out.WriteLine();

            return null;
        }
    }
}
