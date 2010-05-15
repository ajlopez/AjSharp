using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

namespace AjSharp.RunHost
{
    [ServiceContract]
    public interface IRunServer
    {
        [OperationContract]
        byte[] Evaluate(byte[] serializedexpr);

        [OperationContract]
        void Execute(byte[] serializedcmd);
    }
}
