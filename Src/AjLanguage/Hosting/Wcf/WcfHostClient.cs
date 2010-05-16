using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using AjLanguage.Commands;
using AjLanguage.Expressions;
using AjLanguage.Language;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace AjLanguage.Hosting.Wcf
{
    public class WcfHostClient : IHost
    {
        private IHostServer server;
        private BinaryFormatter formatter;

        public WcfHostClient(string address)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<IHostServer> factory = new ChannelFactory<IHostServer>(binding, new EndpointAddress(address.Substring(1)));
            this.server = factory.CreateChannel();
        }

        public Guid Id
        {
            get { throw new NotImplementedException(); }
        }

        public void Execute(ICommand command)
        {
            MemoryStream stream = new MemoryStream();
            this.formatter.Serialize(stream, command);
            stream.Close();
            this.server.Execute(stream.ToArray());
        }

        public object Evaluate(IExpression expression)
        {
            MemoryStream stream = new MemoryStream();
            this.formatter.Serialize(stream, expression);
            stream.Close();
            byte[] data = this.server.Evaluate(stream.ToArray());
            return this.formatter.Deserialize(new MemoryStream(data));
        }

        public object Invoke(IObject receiver, string name, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public object Invoke(Guid receiver, string name, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public object ResultToObject(object result)
        {
            throw new NotImplementedException();
        }
    }
}
