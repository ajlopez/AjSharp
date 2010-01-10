namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class Future : IReference
    {
        private object value;
        private ManualResetEvent handle = new ManualResetEvent(false);
        private bool set = false;

        public void SetValue(object value)
        {
            lock (this)
            {
                if (this.set)
                    throw new InvalidOperationException("Future value already calculated");
                set = true;
                this.value = value;
                this.handle.Set();
            }
        }

        public object GetValue()
        {
            this.handle.WaitOne();
            return this.value;
        }
    }
}
