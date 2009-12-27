namespace AjLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BindingEnvironment : AjLanguage.IBindingEnvironment
    {
        private IBindingEnvironment parent;
        private Dictionary<string, object> values = new Dictionary<string, object>();

        public BindingEnvironment()
        {
        }

        public BindingEnvironment(IBindingEnvironment parent)
        {
            this.parent = parent;
        }

        protected IBindingEnvironment Parent { get { return this.parent; } }

        protected Dictionary<string, object> Values { get { return this.values; } }

        public virtual object GetValue(string name)
        {
            if (!this.values.ContainsKey(name))
            {
                if (this.parent != null)
                    return this.parent.GetValue(name);

                return null;
            }

            return this.values[name];
        }

        public virtual void SetValue(string name, object value)
        {
            this.values[name] = value;
        }

        public virtual bool ContainsName(string name)
        {
            return this.values.ContainsKey(name);
        }

        public virtual void SetLocalValue(string name, object value)
        {
            this.values[name] = value;
        }
    }
}
