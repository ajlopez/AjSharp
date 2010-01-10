namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class Channel : IChannel, IReference
    {
        private AutoResetEvent sethandle = new AutoResetEvent(false);
        private AutoResetEvent gethandle = new AutoResetEvent(false);
        private object value;

        public void Send(object value)
        {
            lock (this.sethandle)
            {
                this.gethandle.WaitOne();
                this.value = value;
                this.sethandle.Set();
            }
        }

        public object Receive()
        {
            lock (this.gethandle)
            {
                this.gethandle.Set();
                this.sethandle.WaitOne();

                object result = this.value;
                return result;
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

