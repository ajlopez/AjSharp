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
    using AjSharp.Primitives;

    public class Program
    {
        public static void Main(string[] args)
        {
            AjSharpMachine machine = new AjSharpMachine();
            Parser parser;
            ICommand command;

            foreach (string filename in args)
            {
                parser = new Parser(System.IO.File.OpenText(filename));

                while ((command = parser.ParseCommand()) != null)
                    command.Execute(machine.Environment);
            }

            parser = new Parser(machine.In);

            command = parser.ParseCommand();

            while (command != null)
            {
                command.Execute(machine.Environment);
                command = parser.ParseCommand();
            }
        }
    }
}
