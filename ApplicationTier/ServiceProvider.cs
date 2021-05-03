using CsharpVersion;
using HistoryDemo;
using Microsoft.AspNetCore.SignalR.Client;
using ModelEntities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ApplicationTier
{
    public interface IServiceProvider
    {
        public Simulation Simulation { get; set; }
        public ConcurrentQueue<double> DataQueue { get; set; }
        public System.Timers.Timer DataSendTimer { get; set; }
        public HubConnection Connection { get; set; }

        public void StartHubConnection(string hubUrl);

        public Task SendData();

        public void StartSimulation();
        public void Reset();

        public void SaveToDatabase(Initialization ini, ModelEntities.Configuration conf);

    }

    class ServiceProvider : IServiceProvider
    {
        public Simulation Simulation { get; set; }
        public ConcurrentQueue<double> DataQueue { get; set; }
        public System.Timers.Timer DataSendTimer { get; set; }
        public HubConnection Connection { get; set; }

        public ServiceProvider()
        {
            Simulation = new Simulation();
            DataSendTimer = new System.Timers.Timer(100);
            DataSendTimer.Elapsed += (sender, e) =>
            {
                if (DataQueue.TryDequeue(out double data))
                {
                    Connection.InvokeAsync("SendData", "user", data);
                }
                if (DataQueue.IsEmpty)
                {
                    DataSendTimer.Stop();
                    Console.WriteLine("Timer Stoped");
                }
            };
            DataQueue = new ConcurrentQueue<double>();
            //Simulation.Record.SaveToDatabase += SaveToDatabase;
        }

        ~ServiceProvider()
        {
            DataSendTimer.Dispose();
        }

        async public void StartHubConnection(string hubUrl)
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();
            await Connection.StartAsync();
            Console.WriteLine(Connection.State);
            if (Connection.State == HubConnectionState.Connected)
            {
                Console.WriteLine("connection started");
            }
        }

        public Task SendData()
        {
            return new Task(() =>
            {
                if (DataQueue.TryDequeue(out double data))
                {
                    Connection.InvokeAsync("SendData", "user", data);
                }
            });
        }

        public void StartSimulation()
        {
            var ini = new Initialization()
            {
                InitialPositionX = Simulation.Plane.Position[0],
                InitialPositionY = Simulation.Plane.Position[1],
                InitialPositionZ = Simulation.Plane.Position[2],
                InitialAttitudePhi = Simulation.Plane.Phi,
                InitialAttitudePsi = Simulation.Plane.Psi,
                InitialAttitudeTheta = Simulation.Plane.Theta
            };
            //var conf = new ModelEntities.Configuration()
            //{
            //    GuidanceController = CsharpVersion.Configuration.GuidanceController
            //};
            DataSendTimer.Start();
            Simulation.Simulate(DataQueue);
            Console.WriteLine(Simulation.Plane.Position.ToString("G40"));
            //SaveToDatabase(ini, Simulation.Configuration);
        }

        async public void SaveToDatabase(Initialization ini, ModelEntities.Configuration conf)
        {
            string fileName = $"datalog\\{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.mat";
            await Task.Run(() =>
            {
                Console.WriteLine(fileName);
                Simulation.Record.SaveToFile(fileName);
                DataManipulation.Create(ini, conf, fileName);
            });
        }

        public void Reset()
        {
            Simulation.Reset();
        }
    }
}
