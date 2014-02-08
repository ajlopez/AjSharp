namespace AjSharp.RunHost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;

    [ServiceContract]
    public interface IRunServer
    {
        [OperationContract]
        byte[] Evaluate(byte[] serializedexpr);

        [OperationContract]
        void Execute(byte[] serializedcmd);
    }
}
