using ApplicationTier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject1
{
    [TestClass]
    public class TestServiceProvider
    {
        [TestMethod]
        public void TestDataSending()
        {
            SimulationWrapper serviceProvider = new();
            serviceProvider.StartSimulation();
        }

        [TestMethod]
        public void TestNewSimulation()
        {
            SimulationWrapper serviceProvider = new();
            //serviceProvider.DataSendTimer.Elapsed -= serviceProvider.SendData;
            //serviceProvider.DataSendTimer.Elapsed += serviceProvider.SendData2;
            serviceProvider.StartSimulation2();
            //Thread.Sleep(5000);
        }
    }
}
