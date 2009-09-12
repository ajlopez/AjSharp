using System;
namespace AjLanguage
{
    public interface IBindingEnvironment
    {
        object GetValue(string name);
        void SetValue(string name, object value);
    }
}
