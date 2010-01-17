namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class QueueChannel : IChannel, IReference
    {
        private Queue<object> values = new Queue<object>();
        private int maxsize;

        public QueueChannel(int maxsize)
        {
            if (maxsize <= 0)
                throw new InvalidOperationException("QueueChannel needs a positive maxsize");

            this.maxsize = maxsize;
        }

        public void Send(object value)
        {
            lock (this)
            {
                while (this.values.Count >= this.maxsize)
                    Monitor.Wait(this);

                this.values.Enqueue(value);
                Monitor.PulseAll(this);
            }
        }

        public object Receive()
        {
            lock (this)
            {
                while (this.values.Count == 0)
                    Monitor.Wait(this);

                object value = this.values.Dequeue();
                Monitor.PulseAll(this);
                return value;
            }
        }

        void IReference.SetValue(object value)
        {
            this.Send(value);
        }

        object IReference.GetValue()
        {
            return this.Receive();
        }
    }
}

