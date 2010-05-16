using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AjLanguage.Hosting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting;
using AjSharp.Compiler;
using AjLanguage.Commands;
using AjLanguage.Language;
using System.Runtime.Remoting.Channels;
using System.Collections;
using AjLanguage.Hosting.Remoting;

namespace AjSharp.RemotingHost
{
    class Program
    {
        static IList<IHost> localhosts = new List<IHost>();
        static IList<IHost> remotehosts = new List<IHost>();

        static void Main(string[] args)
        {
            foreach (string arg in args)
                if (IsLocalAddress(arg))
                    ProcessLocalAddress(arg);
                else
                    ProcessRemoteAddress(arg);

            foreach (IHost host in remotehosts)
            {
                Parser parser = new Parser("PrintLine(\"Hello, world\");");
                ICommand cmd = parser.ParseCommand();
                host.Execute(cmd);

                parser = new Parser("new DynamicObject()");
                IObject result = (IObject)host.Evaluate(parser.ParseExpression());
                host.Invoke(result, "SetValue", new object[] { "Print", new Function(null, cmd) });
                object func = host.Invoke(result, "GetValue", new object[] { "Print" });
                host.Invoke(result, "Print", null);
                // Function function = new Function(null, cmd);
                //result.SetValue("Print", function);
                //result.Invoke("Print", null);
            }

            Console.ReadLine();
        }

        static bool IsLocalAddress(string arg)
        {
            return char.IsDigit(arg[0]);
        }

        static void ProcessLocalAddress(string arg)
        {
            string[] parts = arg.Split(':');
            int port = Convert.ToInt32(parts[0]);
            string name = parts[1];

            localhosts.Add(new RemotingHostServer(new AjSharpMachine(), port, name));

            //// According to http://www.thinktecture.com/resourcearchive/net-remoting-faq/changes2003
            //// in order to have ObjRef accessible from client code
            //BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
            //serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            //BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();

            //IDictionary props = new Hashtable();
            //props["port"] = port;

            //TcpChannel channel = new TcpChannel(props, clientProv, serverProv);
            //// end of "according"

            //Host host = new Host(new AjSharpMachine());
            //RemotingServices.Marshal(host, name);

            ////ChannelServices.RegisterChannel(channel, false);
            ////RemotingConfiguration.RegisterActivatedServiceType(typeof(MyHost));

            //localhosts.Add(host);
        }

        static void ProcessRemoteAddress(string address)
        {
            remotehosts.Add(new RemotingHostClient(address));
            //remotehosts.Add((IHost)Activator.GetObject(typeof(IHost), address));

            // Other attempts
            //remotehosts.Add((IHost)RemotingServices.Connect(typeof(Host), address));
            //RemotingConfiguration.RegisterActivatedClientType(typeof(MyHost), address);
            //remotehosts.Add(new MyHost());
        }
    }

    class MyHost : Host
    {
        public MyHost()
            : base(new AjSharpMachine())
        {
        }
    }
}
