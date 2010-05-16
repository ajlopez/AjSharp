namespace AjLanguage.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Language;

    [Serializable]
    public class ObjectProxy : IObject
    {
        [NonSerialized] private IHost host;
        [NonSerialized] private object obj;

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
            return this.Invoke("GetValue", new object[] { name });
        }

        public void SetValue(string name, object value)
        {
            this.Invoke("SetValue", new object[] { name, value });
        }

        public ICollection<string> GetNames()
        {
            throw new NotImplementedException();
        }

        public object Invoke(string name, object[] parameters)
        {
            if (this.host == null)
                this.host = Machine.Current.GetHost(this.HostId);

            return this.host.Invoke(this, name, parameters);
        }

        public object Invoke(ICallable method, object[] parameters)
        {
            if (this.host == null)
                this.host = Machine.Current.GetHost(this.HostId);

            return this.host.Invoke(this, method, parameters);
        }
    }
}
