namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AgentObject : DynamicClassicObject
    {
        public AgentObject(IClass objclass) 
            : base(objclass)
        {
        }

        // TODO Invoke via agent queue
        public override object Invoke(ICallable method, object[] parameters)
        {
            IBindingEnvironment objenv = new ObjectEnvironment(this, method.Environment);

            return method.Invoke(objenv, parameters);
        }
    }
}
