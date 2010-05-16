namespace AjLanguage.Hosting.Remoting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Language;

    public class ClientObject : IObject
    {
        private IHost host;
        private IObject obj;

        public ClientObject(IHost host, IObject obj)
        {
            this.host = host;
            this.obj = obj;
        }

        public object GetValue(string name)
        {
            return this.host.Invoke(this.obj, "GetValue", new object[] { name });
        }

        public void SetValue(string name, object value)
        {
            this.host.Invoke(this.obj, "SetValue", new object[] { name , value});
        }

        public ICollection<string> GetNames()
        {
            return (ICollection<string>) this.host.Invoke(this.obj, "GetNames", new object[] { });
        }

        public object Invoke(string name, object[] parameters)
        {
            return this.host.Invoke(this.obj, name, parameters);
        }

        public object Invoke(ICallable method, object[] parameters)
        {
            return this.host.Invoke(this.obj, method, parameters);
        }
    }
}
