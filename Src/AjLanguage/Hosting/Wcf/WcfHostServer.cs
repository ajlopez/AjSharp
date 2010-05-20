using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization.Formatters.Binary;
using AjLanguage.Commands;
using System.IO;
using AjLanguage.Expressions;

namespace AjLanguage.Hosting.Wcf
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public class WcfHostServer : Host, IHostServer
    {
        private BinaryFormatter formatter;
        private ServiceHost service;
        private string address;

        public WcfHostServer(string address)
            : this(new Machine(false), address)
        {
        }

        public WcfHostServer(Machine machine, string address)
            : base(machine)
        {
            this.service = new ServiceHost(this);
            this.address = address;
            BasicHttpBinding binding = new BasicHttpBinding();
            this.service.AddServiceEndpoint(typeof(IHostServer), binding, address);
            this.formatter = new BinaryFormatter();
        }

        public override string Address { get { return this.address; } }

        public void Open()
        {
            this.service.Open();
        }

        public void Close()
        {
            this.service.Close();
        }

        public void ExecuteCommand(string commandtext)
        {
            this.Execute(commandtext);
        }

        public byte[] Evaluate(byte[] serializedexpr)
        {
            IExpression expression = (IExpression)this.formatter.Deserialize(new MemoryStream(serializedexpr));
            object result = this.Evaluate(expression);

            if (result is MarshalByRefObject)
                result = this.ResultToObject(result);

            MemoryStream stream = new MemoryStream();
            this.formatter.Serialize(stream, result);
            stream.Close();
            return stream.ToArray();
        }

        public void Execute(byte[] serializedcmd)
        {
            ICommand command = (ICommand)this.formatter.Deserialize(new MemoryStream(serializedcmd));
            this.Execute(command);
        }

        public Guid GetId()
        {
            return this.Id;
        }

        public override void RegisterHost(string address)
        {
            IHost client = new WcfHostClient(address);
            this.Machine.RegisterHost(client);
        }
    }
}
