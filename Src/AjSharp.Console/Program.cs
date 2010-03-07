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
    using AjLanguage.Language;

    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AjSharpMachine machine = new AjSharpMachine();
            Parser parser;
            ICommand command;

            foreach (string filename in args)
            {
                try
                {
                    parser = new Parser(System.IO.File.OpenText(filename));

                    while ((command = parser.ParseCommand()) != null)
                        command.Execute(machine.Environment);
                }
                catch (ExitException ex)
                {
                    return;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        Console.Error.WriteLine(ex.InnerException.Message);
                        Console.Error.WriteLine(ex.InnerException.StackTrace);
                    }
                    Console.Error.WriteLine(ex.Message);
                    Console.Error.WriteLine(ex.StackTrace);
                }
            }

            try
            {
                parser = new Parser(machine.In);

                command = parser.ParseCommand();

                while (command != null)
                {
                    command.Execute(machine.Environment);
                    command = parser.ParseCommand();
                }
            }
            catch (ExitException ex)
            {
                return;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Console.Error.WriteLine(ex.InnerException.Message);
                    Console.Error.WriteLine(ex.InnerException.StackTrace);
                }
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
