namespace AjLanguage.Transactions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Language;

    public interface ITransactionalReference : IReference
    {
        void SetValue(object value, Transaction transaction);
        object GetValue(Transaction transaction);
        void Complete(Transaction transaction, long timestamp);
        void Dispose(Transaction transaction);
    }
}
