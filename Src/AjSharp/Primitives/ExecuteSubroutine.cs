namespace AjSharp.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;

    using AjSharp.Compiler;

    public class ExecuteSubroutine : ICallable
    {
        public int Arity { get { return 1; } }

        public object Invoke(IBindingEnvironment environment, object[] arguments)
        {
            if (arguments == null || arguments.Length != 1)
                throw new InvalidOperationException("Invalid number of parameters");

            string text = (string)arguments[0];

            Parser parser = new Parser(text);

            ICommand command;

            while ((command = parser.ParseCommand()) != null)
                command.Execute(environment);

            return null;
        }

        public object Invoke(object[] arguments)
        {
            return this.Invoke(Machine.Current.Environment, arguments);
        }
    }
}
