namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ITransactionalReference : IReference
    {
        void SetValue(object value, Transaction transaction);
        object GetValue(Transaction transaction);
        void Complete(Transaction transaction);
        void Dispose(Transaction transaction);
    }
}
