namespace AjSharp.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage;
    using AjLanguage.Commands;
    using AjLanguage.Primitives;

    using AjSharp.Compiler;

    class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser(Console.In);

            BindingEnvironment environment = new BindingEnvironment();
            environment.SetValue("Print", new PrintSubroutine());
            environment.SetValue("PrintLine", new PrintLineSubroutine());

            ICommand command;

            command = parser.ParseCommand();

            while (command != null)
            {
                command.Execute(environment);
                command = parser.ParseCommand();
            }
        }
    }
}
