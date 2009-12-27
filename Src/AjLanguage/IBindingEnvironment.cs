namespace AjLanguage
{
    using System;

    public interface IBindingEnvironment
    {
        object GetValue(string name);

        void SetValue(string name, object value);

        bool ContainsName(string name);

        void SetLocalValue(string name, object value);
    }
}
