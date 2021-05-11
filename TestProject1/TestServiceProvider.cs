using ApplicationTier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
