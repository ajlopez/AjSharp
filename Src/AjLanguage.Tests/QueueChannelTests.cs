using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AjLanguage.Language;
using System.Threading;

namespace AjLanguage.Tests
{
    [TestClass]
    public class QueueChannelTests
    {
        [TestMethod]
        public void CreateAndUseQueueChannel()
        {
            QueueChannel channel = new QueueChannel(1);

            Thread thread = new Thread(new ThreadStart(delegate() { channel.Send(10); }));
            thread.Start();

            object result = channel.Receive();
        }

        [TestMethod]
        public void CreateAndUseQueueChannelTenTimes()
        {
            QueueChannel channel = new QueueChannel(1);

            Thread thread = new Thread(new ThreadStart(delegate() { for (int k=1; k<=10; k++) channel.Send(k); }));
            thread.Start();

            for (int j=1; j<=10; j++)
                Assert.AreEqual(j, channel.Receive());
        }

        [TestMethod]
        public void CreateAndUseQueueChannelWithTenElements()
        {
            QueueChannel channel = new QueueChannel(10);

            for (int k = 1; k <= 10; k++)
                channel.Send(k);

            for (int k = 1; k <= 10; k++)
                Assert.AreEqual(k, channel.Receive());
        }

        [TestMethod]
        public void CreateAndUseQueueChannelWithMoreEntriesThanSize()
        {
            QueueChannel channel = new QueueChannel(10);

            Thread thread = new Thread(new ThreadStart(delegate()
            {
                for (int k = 1; k <= 20; k++)
                    channel.Send(k);
            }));

            thread.Start();

            for (int k = 1; k <= 20; k++)
                Assert.AreEqual(k, channel.Receive());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseIfZeroInConstructor()
        {
            new QueueChannel(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseIfNegativeNumberInConstructor()
        {
            new QueueChannel(-1);
        }
    }
}
