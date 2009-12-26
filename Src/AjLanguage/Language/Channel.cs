namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class Channel
    {
        private AutoResetEvent sethandle = new AutoResetEvent(false);
        private AutoResetEvent gethandle = new AutoResetEvent(false);
        private object value;

        public void Send(object value)
        {
            gethandle.WaitOne();
            this.value = value;
            sethandle.Set();
        }

        public object Receive()
        {
            gethandle.Set();
            sethandle.WaitOne();

            object result = this.value;
            return result;
        }
    }
}
