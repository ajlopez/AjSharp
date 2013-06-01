namespace AjLanguage.Hosting.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;

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

        [OperationContract]
        void ExecuteCommand(string commandtext);
    }
}
