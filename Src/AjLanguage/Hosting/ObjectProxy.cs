namespace AjLanguage.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Language;

    public class ObjectProxy : IObject
    {
        private IHost host;
        private object obj;

        public ObjectProxy(object obj, IHost host)
        {
            this.obj = obj;
            this.HostId = host.Id;
            this.ObjectId = Guid.NewGuid();
        }

        public object Object { get { return this.obj; } }

        public Guid HostId { get; set; }
        public Guid ObjectId { get; set; }

        public object GetValue(string name)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string name, object value)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetNames()
        {
            throw new NotImplementedException();
        }

        public object Invoke(string name, object[] parameters)
        {
            if (this.host == null)
                this.host = Machine.Current.GetHost(this.HostId);

            return this.host.Invoke(this.ObjectId, name, parameters);
        }

        public object Invoke(ICallable method, object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
