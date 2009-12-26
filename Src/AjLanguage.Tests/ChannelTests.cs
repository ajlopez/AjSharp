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
    public class ChannelTests
    {
        [TestMethod]
        public void CreateAndUseChannel()
        {
            Channel channel = new Channel();

            Thread thread = new Thread(new ThreadStart(delegate() { channel.Send(10); }));
            thread.Start();

            object result = channel.Receive();
        }

        [TestMethod]
        public void CreateAndUseChannelTenTimes()
        {
            Channel channel = new Channel();

            Thread thread = new Thread(new ThreadStart(delegate() { for (int k=1; k<=10; k++) channel.Send(k); }));
            thread.Start();

            for (int j=1; j<=10; j++)
                Assert.AreEqual(j, channel.Receive());
        }
    }
}
