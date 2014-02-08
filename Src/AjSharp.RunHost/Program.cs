﻿namespace AjSharp.RunHost
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.ServiceModel;
    using System.Text;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Hosting.Wcf;
    using AjLanguage.Language;
    using AjSharp.Compiler;

    public class Program
    {
        private static IList<WcfHostServer> servers = new List<WcfHostServer>();
        private static IList<WcfHostClient> channels = new List<WcfHostClient>();

        public static void Main(string[] args)
        {
            foreach (string address in args)
            {
                if (address[0] == '-')
                    continue;
                
                servers.Add(new WcfHostServer(address));
            }

            foreach (WcfHostServer server in servers)
                server.Open();

            foreach (string address in args)
            {
                if (address[0] != '-')
                    continue;

                channels.Add(new WcfHostClient(address.Substring(1)));
            }

            try
            {
                Parser parser = new Parser("new DynamicObject()");

                IExpression expression = parser.ParseExpression();

                if (channels.Count > 0)
                {
                    object result = channels[0].Evaluate(expression);
                }

                parser = new Parser(System.Console.In);

                ICommand command = parser.ParseCommand();

                while (command != null)
                {
                    channels[0].Execute(command);
                    command = parser.ParseCommand();
                }
            }
            catch (ExitException)
            {
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

            foreach (WcfHostServer server in servers)
                server.Close();
        }
    }
}
