namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IClass
    {
        void SetMember(string name, object value);

        object GetMember(string name);

        object NewInstance(object[] parameters);

        ICollection<string> GetMemberNames();
    }
}
