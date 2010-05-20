namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class DynamicObject : IObject
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();
        private static string[] nativeMethods = new string[] { "SetValue", "GetValue", "ToString", "GetNames", "Invoke", "GetHashCode", "Equals", "Marshal" };

        public virtual void SetValue(string name, object value)
        {
            this.values[name] = value;
        }

        public virtual object GetValue(string name)
        {
            if (this.values.ContainsKey(name))
                return this.values[name];

            return null;
        }

        public virtual ICollection<string> GetNames()
        {
            return this.values.Keys;
        }

        public virtual bool IsNativeMethod(string name)
        {
            return nativeMethods.Contains(name);
        }

        public virtual IObject Marshal()
        {
            return new ProxyServerObject(this);
        }

        public virtual object Invoke(string name, object[] parameters)
        {
            object value = this.GetValue(name);

            if (value == null && this.IsNativeMethod(name))
                return ObjectUtilities.GetNativeValue(this, name, parameters);

            if (value == null)
                throw new InvalidOperationException(string.Format("Unknown member '{0}'", name));

            if (!(value is ICallable))
            {
                if (parameters == null)
                    return value;

                throw new InvalidOperationException(string.Format("'{0}' is not a method", name));
            }

            ICallable method = (ICallable)value;

            IBindingEnvironment objenv = new ObjectEnvironment(this, method.Environment);

            return method.Invoke(objenv, parameters);
        }

        public virtual object Invoke(ICallable method, object[] parameters)
        {
            IBindingEnvironment objenv = new ObjectEnvironment(this, method.Environment);

            return method.Invoke(objenv, parameters);
        }
    }
}
