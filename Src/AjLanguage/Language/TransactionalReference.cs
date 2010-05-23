using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AjLanguage.Language
{
    public class TransactionalReference : ITransactionalReference
    {
        private object value;
        private Machine machine;

        public TransactionalReference()
        {
            this.machine = Machine.Current;
        }

        public void SetValue(object value, Transaction transaction)
        {
            if (transaction.Machine != this.machine)
                throw new InvalidOperationException("Transaction is from another Machine");

            throw new NotImplementedException();
        }

        public object GetValue(Transaction transaction)
        {
            if (transaction.Machine != this.machine)
                throw new InvalidOperationException("Transaction is from another Machine");

            throw new NotImplementedException();
        }

        public void Complete(Transaction transaction)
        {
            if (transaction.Machine != this.machine)
                throw new InvalidOperationException("Transaction is from another Machine");

            throw new NotImplementedException();
        }

        public void Dispose(Transaction transaction)
        {
            if (transaction.Machine != this.machine)
                throw new InvalidOperationException("Transaction is from another Machine");

            throw new NotImplementedException();
        }

        public void SetValue(object value)
        {
            if (Machine.CurrentTransaction != null)
                this.SetValue(value, Machine.CurrentTransaction);
            else
                this.value = value;
        }

        public object GetValue()
        {
            if (Machine.CurrentTransaction != null)
                return this.GetValue(Machine.CurrentTransaction);

            return this.value;
        }
    }
}
