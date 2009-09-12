namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ICallable
    {
        object Invoke(IBindingEnvironment environment, object[] arguments);

        int Arity { get; }
    }
}
