namespace AjLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Language;

    public class ObjectEnvironment : IBindingEnvironment
    {
        public const string ThisName = "this";

        private IBindingEnvironment parent;
        private DynamicObject obj;

        public ObjectEnvironment(DynamicObject obj)
            : this(obj, null)
        {
        }

        public ObjectEnvironment(DynamicObject obj, IBindingEnvironment parent)
        {
            this.obj = obj;
            this.parent = parent;
        }

        public object GetValue(string name)
        {
            if (name == ThisName)
                return this.obj;

            object result = this.obj.GetValue(name);

            if (result == null && this.parent != null)
                return this.parent.GetValue(name);

            return result;
        }

        public void SetValue(string name, object value)
        {
            this.obj.SetValue(name, value);
        }

        public void SetLocalValue(string name, object value)
        {
            throw new NotSupportedException();
        }

        public bool ContainsName(string name)
        {
            return this.obj.GetNames().Contains(name);
        }
    }
}
