using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization.Formatters.Binary;
using AjLanguage.Commands;
using AjLanguage.Expressions;
using AjLanguage;
using System.IO;
using AjLanguage.Hosting;

namespace AjSharp.RunHost
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
    public class RunServer : IRunServer
    {
        private Host host;
        private Machine machine;
        private ServiceHost service;
        private BinaryFormatter formatter;

        public RunServer(string address)
        {
            this.machine = new AjSharpMachine(false);
            this.host = new Host(this.machine);
            this.service = new ServiceHost(this);
            BasicHttpBinding binding = new BasicHttpBinding();
            this.service.AddServiceEndpoint(typeof(IRunServer), binding, address);
            this.formatter = new BinaryFormatter();
        }

        public void Open()
        {
            this.service.Open();
        }

        public void Close()
        {
            this.service.Close();
        }

        public void Execute(ICommand command)
        {
            this.machine.SetCurrent();
            command.Execute(this.machine.Environment);
        }

        public object Evaluate(IExpression expression)
        {
            this.machine.SetCurrent();
            object result = expression.Evaluate(this.machine.Environment);

            if (!(result is MarshalByRefObject))
                return result;
            
            return this.host.ResultToObject(result);
        }

        public void Execute(byte[] serializedcmd)
        {
            ICommand command = (ICommand) this.formatter.Deserialize(new MemoryStream(serializedcmd));
            this.Execute(command);
        }

        public byte[] Evaluate(byte[] serializedexpr)
        {
            IExpression expression = (IExpression)this.formatter.Deserialize(new MemoryStream(serializedexpr));
            object result = this.Evaluate(expression);
            MemoryStream stream = new MemoryStream();
            this.formatter.Serialize(stream, result);
            stream.Close();
            return stream.ToArray();
        }
    }
}
