namespace AjLanguage.Transactions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using AjLanguage.Language;

    public class Transaction : IDisposable
    {
        private static long nextId = 0;
        private Machine machine;

        private long id;
        private HashSet<IReference> references = new HashSet<IReference>();

        public Transaction(Machine machine)
        {
            this.id = NextId();

            NextId(); // for out of transaction get,set

            if (machine == null)
                throw new InvalidOperationException("Transaction should run in a Machine");

            this.machine = machine;

            machine.RegisterTransaction(this);
        }

        public long Id { get { return this.id; } }

        public Machine Machine { get { return this.machine; } }

        public static long NextId()
        {
            return Interlocked.Increment(ref nextId);
        }

        public static long CurrentId()
        {
            return Interlocked.Read(ref nextId);
        }

        public void Complete()
        {
            lock (this)
            {
                long timestamp = NextId();

                foreach (ITransactionalReference reference in this.references)
                    reference.Complete(this, timestamp);

                references.Clear();
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                foreach (ITransactionalReference reference in this.references)
                    reference.Dispose(this);

                references.Clear();
            }

            this.machine.UnregisterTransaction(this);
        }

        public void RegisterTransactionalReference(IReference reference)
        {
            lock (this)
            {
                if (!this.references.Contains(reference))
                    this.references.Add(reference);
            }
        }

        public static bool IsPrevious(long timestamp1, long timestamp2)
        {
            return (timestamp2 - timestamp1) > 0;
        }
    }
}

