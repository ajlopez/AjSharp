namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DynamicObject : IObject
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();

        public virtual void SetValue(string name, object value)
        {
            values[name] = value;
        }

        public virtual object GetValue(string name)
        {
            if (values.ContainsKey(name))
                return values[name];

            return null;
        }

        public virtual ICollection<string> GetNames()
        {
            return this.values.Keys;
        }
    }
}
