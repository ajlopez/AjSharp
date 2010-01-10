using System;
using System.Text;
using System.Collections.Generic;
using AjLanguage.Language;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace AjLanguage.Tests
{
    [TestClass]
    public class FutureTests
    {
        [TestMethod]
        public void CreateFutureSetAndGetValue()
        {
            Future future = new Future();

            future.SetValue(1);

            object result = future.GetValue();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);

            result = future.GetValue();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void CreateFutureGetAndSetValue()
        {
            Future future = new Future();
            ManualResetEvent handle = new ManualResetEvent(false);
            bool executed = false;

            Thread thread = new Thread(new ThreadStart(() => { handle.WaitOne(); executed = true;  future.SetValue(1); }));
            thread.Start();

            Assert.IsFalse(executed);
            Thread.Sleep(100);
            Assert.IsFalse(executed);

            handle.Set();

            object result = future.GetValue();
            Assert.IsTrue(executed);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }
    }
}
