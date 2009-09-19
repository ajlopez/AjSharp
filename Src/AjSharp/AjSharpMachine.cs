namespace AjSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage;
    using AjLanguage.Language;
    using AjLanguage.Primitives;

    using AjSharp.Primitives;

    public class AjSharpMachine : Machine
    {
        public AjSharpMachine()
            : this(true)
        {
        }

        public AjSharpMachine(bool iscurrent)
            : base(iscurrent)
        {
            this.Environment.SetValue("DynamicObject", typeof(DynamicObject));
            this.Environment.SetValue("DynamicClass", typeof(DynamicClass));
            this.Environment.SetValue("List", typeof(ArrayList));
            this.Environment.SetValue("Print", new PrintSubroutine());
            this.Environment.SetValue("PrintLine", new PrintLineSubroutine());
            this.Environment.SetValue("Include", new IncludeSubroutine());
        }
    }
}
