namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class DynamicClassicObject : DynamicObject, IClassicObject
    {
        [NonSerialized] private IClass objclass;
        private string classname;

        public DynamicClassicObject(IClass objclass) 
        {
            this.objclass = objclass;
            this.classname = objclass.Name;
        }

        public IClass GetClass()
        {
            return this.objclass;
        }

        public override object Invoke(string name, object[] parameters)
        {
            object value = this.GetValue(name);

            if (value == null || !(value is ICallable))
                value = this.objclass.GetMember(name);

            if (value == null)
            {
                if (this.IsNativeMethod(name))
                    return ObjectUtilities.GetNativeValue(this, name, parameters);

                if (this.objclass.DefaultMember != null)
                {
                    ICallable defmethod = (ICallable) this.objclass.DefaultMember;
                    IBindingEnvironment oenv = new ObjectEnvironment(this, defmethod.Environment);
                    object[] arguments = new object[] { name, parameters };
                    return defmethod.Invoke(oenv, arguments);
                }
                else
                    return base.Invoke(name, parameters);
            }

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
    }
}
