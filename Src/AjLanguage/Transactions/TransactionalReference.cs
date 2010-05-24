using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AjLanguage.Transactions
{
    public class TransactionalReference : ITransactionalReference
    {
        private object value;
        private long timestamp;

        private Transaction pending;
        private object pendingvalue;

        private Dictionary<long, object> snapshots = new Dictionary<long, object>();

        public TransactionalReference()
        {
        }

        public void SetValue(object value, Transaction transaction)
        {
            lock (this)
            {
                if (this.pending != null && this.pending != transaction)
                    throw new InvalidOperationException("Reference changed in another running Transaction");

                this.pending = transaction;
                this.pendingvalue = value;
                transaction.RegisterTransactionalReference(this);
            }
        }

        public object GetValue(Transaction transaction)
        {
            lock (this)
            {
                if (transaction == this.pending)
                    return this.pendingvalue;

                // TODO review snapshots
                if (this.snapshots.Count == 0)
                    return value;

                long? selected = null;

                foreach (long ts in this.snapshots.Keys)
                    if (!Transaction.IsPrevious(transaction.Id, ts))
                        if (!selected.HasValue || Transaction.IsPrevious(selected.Value, ts))
                            selected = ts;

                if (selected.HasValue)
                    return this.snapshots[selected.Value];

                return value;
            }
        }

        public void Complete(Transaction transaction, long timestamp)
        {
            lock (this)
            {
                if (this.pending != transaction)
                    return;

                this.snapshots[this.timestamp] = this.value;

                this.value = this.pendingvalue;
                this.timestamp = timestamp;

                this.pending = null;
                this.pendingvalue = null;
            }
        }

        public void Dispose(Transaction transaction)
        {
            lock (this)
            {
                if (this.pending == transaction)
                {
                    this.pending = null;
                    this.pendingvalue = null;
                }
            }
        }

        public void SetValue(object value)
        {
            lock (this)
            {
                if (Machine.CurrentTransaction != null)
                    this.SetValue(value, Machine.CurrentTransaction);
                else
                {
                    this.value = value;
                    this.timestamp = Transaction.CurrentId();
                }
            }
        }

        public object GetValue()
        {
            lock (this)
            {
                if (Machine.CurrentTransaction != null)
                    return this.GetValue(Machine.CurrentTransaction);

                return this.value;
            }
        }
    }
}
