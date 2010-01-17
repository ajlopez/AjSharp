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
        private IList<string> globals;

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

                if (this.globals != null && this.globals.Contains(name) && Machine.Current != null)
                    return this.GetGlobalValue(name);

                return null;
            }

            return this.values[name];
        }

        public virtual void SetValue(string name, object value)
        {
            // TODO Review: it's to comply with ClassObjectNoThis.ajs
            if (this.parent != null && !this.values.ContainsKey(name) && (this.parent is ObjectEnvironment || this.parent is LocalBindingEnvironment) && this.parent.ContainsName(name))
            {
                this.parent.SetValue(name, value);
                return;
            }

            if (this.globals != null && !this.values.ContainsKey(name) && this.globals.Contains(name) && Machine.Current != null)
            {
                this.SetGlobalValue(name, value);
                return;
            }

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

        public virtual void DefineGlobal(string name)
        {
            if (Machine.Current != null && Machine.Current.Environment == this)
                throw new InvalidOperationException("Cannot define a global at top environment");

            if (this.globals == null)
                this.globals = new List<string>();

            if (this.globals.Contains(name))
                return;

            this.globals.Add(name);
        }

        private object GetGlobalValue(string name)
        {
            return Machine.Current.Environment.GetValue(name);
        }

        private void SetGlobalValue(string name, object value)
        {
            Machine.Current.Environment.SetValue(name, value);
        }
    }
}
