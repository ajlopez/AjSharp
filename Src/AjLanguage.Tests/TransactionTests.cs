using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AjLanguage.Transactions;

namespace AjLanguage.Tests
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void IsPrevious()
        {
            Assert.IsTrue(Transaction.IsPrevious(0, 1));
            Assert.IsFalse(Transaction.IsPrevious(1, 0));
            Assert.IsFalse(Transaction.IsPrevious(1, 1));
            Assert.IsTrue(Transaction.IsPrevious(-1, 0));
            Assert.IsFalse(Transaction.IsPrevious(0, -1));
            Assert.IsTrue(Transaction.IsPrevious(long.MaxValue, long.MinValue));
            Assert.IsFalse(Transaction.IsPrevious(long.MinValue, long.MaxValue));
        }

        [TestMethod]
        public void SetAndGetValueFromReferenceOutOfTransaction()
        {
            TransactionalReference reference = new TransactionalReference();
            Assert.IsNull(reference.GetValue());
            reference.SetValue("foo");
            Assert.AreEqual("foo", reference.GetValue());
            Assert.IsFalse(reference.HasSnapshots);
        }

        [TestMethod]
        public void SetAndGetValueFromReferenceInNotCommitedTransaction()
        {
            Machine machine = new Machine();
            TransactionalReference reference = new TransactionalReference();
            reference.SetValue("foo");
            Assert.AreEqual("foo", reference.GetValue());
            Transaction transaction = new Transaction(machine);
            Machine.CurrentTransaction = transaction;
            reference.SetValue("bar");
            Assert.AreEqual("bar", reference.GetValue());
            transaction.Dispose();
            Machine.CurrentTransaction = null;
            Assert.AreEqual("foo", reference.GetValue());
            Assert.IsFalse(reference.HasSnapshots);
        }

        [TestMethod]
        public void SetAndGetValueUsingTwoTransactions()
        {
            Machine machine = new Machine();
            TransactionalReference reference = new TransactionalReference();
            reference.SetValue("foo");
            Assert.AreEqual("foo", reference.GetValue());
            
            Transaction transaction1 = new Transaction(machine);
            Transaction transaction2 = new Transaction(machine);

            reference.SetValue("bar", transaction1);
            Assert.AreEqual("foo", reference.GetValue(transaction2));
            Assert.AreEqual("bar", reference.GetValue(transaction1));

            transaction1.Complete();
            transaction1.Dispose();

            Assert.IsTrue(reference.HasSnapshots);

            Assert.AreEqual("bar", reference.GetValue());
            Assert.AreEqual("foo", reference.GetValue(transaction2));

            transaction2.Dispose();

            Assert.IsFalse(reference.HasSnapshots);
        }

        [TestMethod]
        public void SetAndGetValueUsingTwoTransactionsAndTwoReference()
        {
            Machine machine = new Machine();
            TransactionalReference reference1 = new TransactionalReference();
            TransactionalReference reference2 = new TransactionalReference();

            reference1.SetValue("foo");
            Assert.AreEqual("foo", reference1.GetValue());
            Assert.IsFalse(reference1.HasSnapshots);
            reference2.SetValue("bar");
            Assert.AreEqual("bar", reference2.GetValue());
            Assert.IsFalse(reference2.HasSnapshots);

            Transaction transaction1 = new Transaction(machine);
            Transaction transaction2 = new Transaction(machine);

            reference1.SetValue("newfoo", transaction1);
            Assert.AreEqual("foo", reference1.GetValue(transaction2));
            Assert.AreEqual("newfoo", reference1.GetValue(transaction1));
            Assert.AreEqual("foo", reference1.GetValue());

            reference2.SetValue("newbar", transaction2);
            Assert.AreEqual("newbar", reference2.GetValue(transaction2));
            Assert.AreEqual("bar", reference2.GetValue(transaction1));
            Assert.AreEqual("bar", reference2.GetValue());

            Assert.IsFalse(reference1.HasSnapshots);
            Assert.IsFalse(reference2.HasSnapshots);

            transaction1.Complete();
            transaction1.Dispose();

            Assert.IsTrue(reference1.HasSnapshots);
            Assert.IsFalse(reference2.HasSnapshots);

            Assert.AreEqual("foo", reference1.GetValue(transaction2));
            Assert.AreEqual("newfoo", reference1.GetValue());

            Assert.AreEqual("bar", reference2.GetValue());
            Assert.AreEqual("newbar", reference2.GetValue(transaction2));

            transaction2.Complete();
            transaction2.Dispose();

            Assert.IsFalse(reference1.HasSnapshots);
            Assert.IsFalse(reference2.HasSnapshots);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseIfSetValueFromTwoTransactions()
        {
            Machine machine = new Machine();
            TransactionalReference reference = new TransactionalReference();
            reference.SetValue("foo");
            Assert.AreEqual("foo", reference.GetValue());

            Transaction transaction1 = new Transaction(machine);
            Transaction transaction2 = new Transaction(machine);

            reference.SetValue("bar", transaction1);
            reference.SetValue("newbar", transaction2);
            transaction1.Complete();
            transaction2.Complete();
        }
    }
}
