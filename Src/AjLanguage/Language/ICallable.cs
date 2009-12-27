namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ICallable
    {
        int Arity { get; }

        object Invoke(IBindingEnvironment environment, object[] arguments);

        object Invoke(object[] arguments);
    }
}
