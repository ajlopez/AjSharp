namespace AjLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BindingEnvironment
    {
        private BindingEnvironment parent;
        private Dictionary<string, object> values = new Dictionary<string, object>();

        public BindingEnvironment()
        {
        }

        public BindingEnvironment(BindingEnvironment parent)
        {
            this.parent = parent;
        }

        public object GetValue(string name)
        {
            if (!this.values.ContainsKey(name))
            {
                if (this.parent != null)
                    return this.parent.GetValue(name);

                return null;
            }

            return this.values[name];
        }

        public void SetValue(string name, object value)
        {
            // TODO improve, it could be too many calls to .IsDefined
            if (!this.values.ContainsKey(name) && this.parent != null && this.parent.IsDefined(name))
            {
                this.parent.SetValue(name, value);
                return;
            }

            this.values[name] = value;
        }

        private bool IsDefined(string name)
        {
            if (this.values.ContainsKey(name))
                return true;

            if (this.parent != null)
                return this.parent.IsDefined(name);

            return false;
        }
    }
}
