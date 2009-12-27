namespace AjLanguage.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MachineTests
    {
        [TestMethod]
        public void CreateCurrentMachine()
        {
            Machine machine = new Machine();

            Assert.IsTrue(machine == Machine.Current);
            Assert.IsNotNull(machine.Environment);
        }

        [TestMethod]
        public void CreateNotCurrentMachine()
        {
            Machine machine = new Machine(false);

            Assert.IsTrue(machine != Machine.Current);
            Assert.IsNotNull(machine.Environment);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseOnDefineGlobalInMachineEnvironment()
        {
            Machine machine = new Machine();
            machine.Environment.DefineGlobal("global");
        }
    }
}
