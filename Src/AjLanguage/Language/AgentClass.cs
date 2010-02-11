namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AgentClass : DynamicClass
    {
        public AgentClass(string name)
            : base(name)
        {
        }

        public override object NewInstance(object[] parameters)
        {
            AgentObject dynobj = new AgentObject(this);

            this.NewInstance(dynobj, parameters);

            dynobj.Launch();

            return dynobj;
        }
    }
}
