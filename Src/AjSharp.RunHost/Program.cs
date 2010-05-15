using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AjSharp.Compiler;
using AjLanguage.Commands;
using AjLanguage.Language;
using System.ServiceModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace AjSharp.RunHost
{
    class Program
    {
        private static IList<RunServer> servers = new List<RunServer>();
        private static IList<IRunServer> channels = new List<IRunServer>();
        private static BinaryFormatter formatter = new BinaryFormatter();

        static void Main(string[] args)
        {
            foreach (string address in args)
            {
                if (address[0] == '-')
                    continue;
                
                servers.Add(new RunServer(address));
            }

            foreach (RunServer server in servers)
                server.Open();

            foreach (string address in args)
            {
                if (address[0] != '-')
                    continue;

                BasicHttpBinding binding = new BasicHttpBinding();
                ChannelFactory<IRunServer> factory = new ChannelFactory<IRunServer>(binding, new EndpointAddress(address.Substring(1)));
                channels.Add(factory.CreateChannel());
            }

            try
            {
                Parser parser = new Parser(System.Console.In);

                ICommand command = parser.ParseCommand();

                while (command != null)
                {
                    MemoryStream stream = new MemoryStream();
                    formatter.Serialize(stream, command);
                    stream.Close();
                    channels[0].Execute(stream.ToArray());
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

            foreach (RunServer server in servers)
                server.Close();
        }
    }
}
