namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class Transaction : IDisposable
    {
        private static long nextId = 0;
        private bool completed = false;
        private Machine machine;

        private long id;
        private HashSet<IReference> references = new HashSet<IReference>();

        public Transaction(Machine machine)
        {
            this.id = Interlocked.Increment(ref nextId);

            if (machine == null)
                throw new InvalidOperationException("Transaction should run in a Machine");

            this.machine = machine;
        }

        public long Id { get { return this.id; } }

        public Machine Machine { get { return this.machine; } }

        public void Complete()
        {
            lock (Machine.Current)
            {
                foreach (ITransactionalReference reference in this.references)
                    reference.Complete(this);

                references.Clear();
            }
        }

        public void Dispose()
        {
            if (!this.completed)
            {
                lock (Machine.Current)
                {
                    foreach (ITransactionalReference reference in this.references)
                        reference.Dispose(this);

                    references.Clear();
                }
            }
        }

        public void RegisterTransactionalReference(IReference reference)
        {
            if (!this.references.Contains(reference))
                this.references.Add(reference);
        }
    }
}

