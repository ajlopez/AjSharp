namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DynamicClass : IClass
    {
        private Dictionary<string, object> members = new Dictionary<string, object>();

        public virtual void SetMember(string name, object value)
        {
            this.members[name] = value;
        }

        public virtual object GetMember(string name)
        {
            if (this.members.ContainsKey(name))
                return this.members[name];

            return null;
        }

        // TODO use parameters
        public virtual object NewInstance(object[] parameters)
        {
            IObject dynobj = new DynamicClassicObject(this);

            foreach (string name in this.members.Keys)
            {
                object member = this.members[name];

                if (!(member is ICallable))
                    dynobj.SetValue(name, member);
            }

            return dynobj;
        }

        public virtual ICollection<string> GetMemberNames()
        {
            return this.members.Keys;
        }
    }
}
