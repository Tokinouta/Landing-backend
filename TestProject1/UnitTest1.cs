using CsharpVersion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelEntities;
using ModelEntities.Enumerations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ConcurrentQueue<DataToSend> cq = new();
            Simulation simulation = new();
            simulation.Simulate(cq);
            Assert.AreEqual(30714, simulation.Step_count);
        }

        [TestMethod]
        public void TestReset()
        {
            ConcurrentQueue<DataToSend> cq = new();
            Simulation sim = new();
            sim.Simulate(cq);
            sim.Reset();
            cq.Clear();
            Simulation sim2 = new();
            Assert.AreEqual(sim2.Plane.Position, sim.Plane.Position);
            Assert.AreEqual(sim2.PositionLoop.U1, sim.PositionLoop.U1, "PositionLoop");
            Assert.AreEqual(sim2.FlightPathLoop.U2, sim.FlightPathLoop.U2, "FlightPathLoop");
            Assert.AreEqual(sim2.AttitudeLoop.U3, sim.AttitudeLoop.U3, "AttitudeLoop");
            Assert.AreEqual(sim2.AngularRateLoop.Uact, sim.AngularRateLoop.Uact, "AngularRateLoop");
            Assert.AreEqual(sim2.PositionLoop.X1, sim.PositionLoop.X1, "PositionLoop");
            Assert.AreEqual(sim2.FlightPathLoop.X2, sim.FlightPathLoop.X2, "FlightPathLoop");
            Assert.AreEqual(sim2.AttitudeLoop.X3, sim.AttitudeLoop.X3, "AttitudeLoop");
            Assert.AreEqual(sim2.AngularRateLoop.X4, sim.AngularRateLoop.X4, "AngularRateLoop");
            sim.Simulate(cq);
            Assert.AreEqual(30714, sim.Step_count);
        }

        [TestMethod]
        public void TestEnumToList()
        {
            var t = GetEnumList<GuidanceConfig>();
            foreach (var item in t)
            {
                Console.WriteLine(item);
            }
        }

        public static List<TEnum> GetEnumList<TEnum>() where TEnum : Enum
            => ((TEnum[])Enum.GetValues(typeof(TEnum))).ToList();
    }
}
