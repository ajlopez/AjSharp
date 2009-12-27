namespace AjLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class LocalBindingEnvironment : BindingEnvironment
    {
        public LocalBindingEnvironment(IBindingEnvironment parent)
            : base(parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
        }

        public override void SetValue(string name, object value)
        {
            if (this.values.ContainsKey(name))
            {
                this.values[name] = value;
                return;
            }

            if (this.parent.ContainsName(name) || this.parent is LocalBindingEnvironment)
            {
                this.parent.SetValue(name, value);
                return;
            }

            this.values[name] = value;
        }

        public override bool ContainsName(string name)
        {
            if (this.values.ContainsKey(name))
                return true;

            return this.parent.ContainsName(name);
        }
    }
}

