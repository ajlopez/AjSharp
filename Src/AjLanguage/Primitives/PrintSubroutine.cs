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
        public object Invoke(BindingEnvironment environemnt, object[] arguments)
        {
            Machine machine = Machine.Current;

            foreach (object argument in arguments)
                machine.Out.Write(argument.ToString());

            return null;
        }
    }
}
