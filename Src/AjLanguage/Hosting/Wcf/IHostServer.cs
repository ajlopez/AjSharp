using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

namespace AjLanguage.Hosting.Wcf
{
    [ServiceContract]
    public interface IHostServer
    {
        [OperationContract]
        byte[] Evaluate(byte[] serializedexpr);

        [OperationContract]
        void Execute(byte[] serializedcmd);

        [OperationContract]
        Guid GetId();

        [OperationContract]
        void RegisterHost(string address);
    }
}
