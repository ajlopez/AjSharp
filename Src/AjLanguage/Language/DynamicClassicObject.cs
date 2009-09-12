namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DynamicClassicObject : DynamicObject, IClassicObject
    {
        private IClass objclass;

        public DynamicClassicObject(IClass objclass) 
        {
            this.objclass = objclass;
        }

        public IClass GetClass()
        {
            return this.objclass;
        }
    }
}
