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
            Machine machine = new Machine();
            Parser parser = new Parser(machine.In);

            machine.Environment.SetValue("Print", new PrintSubroutine());
            machine.Environment.SetValue("PrintLine", new PrintLineSubroutine());

            ICommand command;

            command = parser.ParseCommand();

            while (command != null)
            {
                command.Execute(machine.Environment);
                command = parser.ParseCommand();
            }
        }
    }
}
