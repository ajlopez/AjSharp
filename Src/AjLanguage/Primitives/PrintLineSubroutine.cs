namespace AjLanguage.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Expressions;

    public class PrintLineSubroutine : ICallable
    {
        public object Invoke(BindingEnvironment environemnt, object[] arguments)
        {
            foreach (object argument in arguments)
                Console.Write(argument.ToString());

            Console.WriteLine();

            return null;
        }
    }
}
