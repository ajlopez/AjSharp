namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ProxyServerObject :MarshalByRefObject, IObject
    {
        private IObject obj;

        public ProxyServerObject(IObject obj)
        {
            this.obj = obj;
        }

        public object GetValue(string name)
        {
            return this.obj.GetValue(name);
        }

        public void SetValue(string name, object value)
        {
            this.obj.SetValue(name, value);
        }

        public ICollection<string> GetNames()
        {
            return this.obj.GetNames();
        }

        public object Invoke(string name, object[] parameters)
        {
            return this.obj.Invoke(name, parameters);
        }

        public object Invoke(ICallable method, object[] parameters)
        {
            return this.obj.Invoke(method, parameters);
        }
    }
}
