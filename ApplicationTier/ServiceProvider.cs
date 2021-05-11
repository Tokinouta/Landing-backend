using CsharpVersion;
using HistoryDemo;
using Microsoft.AspNetCore.SignalR.Client;
using ModelEntities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ApplicationTier
{
    public interface IServiceProvider
    {
        public Simulation Simulation { get; set; }
        public ConcurrentQueue<DataToSend> DataQueue { get; set; }
        public System.Timers.Timer DataSendTimer { get; set; }
        public HubConnection Connection { get; set; }

        public void StartHubConnection(string hubUrl);
        public Task SendData();
        public void StartSimulation();
        public void Reset();
        public void SaveToDatabase(Initialization ini, ModelEntities.Configuration conf);

    }

    public class SimulationWrapper : IServiceProvider
    {
        public Simulation Simulation { get; set; }
        public ConcurrentQueue<DataToSend> DataQueue { get; set; }
        public System.Timers.Timer DataSendTimer { get; set; }
        public HubConnection Connection { get; set; }
        UdpClient udpClient;

        public SimulationWrapper()
        {
            Simulation = new Simulation();
            udpClient = new();
            Simulation.DataSending += () =>
            {
                var sendBytes = Simulation.DataToSend;
                udpClient.Send(sendBytes, sendBytes.Length, "localhost", 33333);
            };
            DataSendTimer = new System.Timers.Timer(100);
            DataSendTimer.Elapsed += (sender, e) =>
            {
                if (DataQueue.TryDequeue(out DataToSend data))
                {
                    Connection.InvokeAsync("SendData", "user", data);
                }
                if (DataQueue.IsEmpty)
                {
                    DataSendTimer.Stop();
                    Console.WriteLine("Timer Stoped");
                }
            };
            DataQueue = new ConcurrentQueue<DataToSend>();
            //Simulation.Record.SaveToDatabase += SaveToDatabase;
        }

        ~SimulationWrapper()
        {
            DataSendTimer.Dispose();
        }

        async public void StartHubConnection(string hubUrl)
        {
            if (Connection?.State == HubConnectionState.Connected)
            {
                Console.WriteLine("connection already started");
            }
            else
            {
                Connection = new HubConnectionBuilder()
                              .WithUrl(hubUrl)
                              .Build();
                await Connection.StartAsync();
                Console.WriteLine(Connection.State);
                Console.WriteLine("connection started");
            }
        }

        public Task SendData()
        {
            return new Task(() =>
            {
                if (DataQueue.TryDequeue(out DataToSend data))
                {
                    Connection.InvokeAsync("SendData", "user", data);
                }
            });
        }

        public void StartSimulation()
        {
            var ini = new Initialization()
            {
                X = Simulation.Plane.Position[0],
                Y = Simulation.Plane.Position[1],
                Z = Simulation.Plane.Position[2],
                Phi = Simulation.Plane.Phi,
                Psi = Simulation.Plane.Psi,
                Theta = Simulation.Plane.Theta,
                P = Simulation.Plane.P,
                Q = Simulation.Plane.Q,
                R = Simulation.Plane.R,
                Alpha = Simulation.Plane.Alpha
            };
            //var conf = new ModelEntities.Configuration()
            //{
            //    GuidanceController = CsharpVersion.Configuration.GuidanceController
            //};
            DataSendTimer.Start();
            Simulation.Simulate(DataQueue);
            Console.WriteLine(Simulation.Plane.Position.ToString("G40"));
            SaveToDatabase(ini, Simulation.Configuration);
            Thread.Sleep(5000);
        }

        async public void SaveToDatabase(Initialization ini, Configuration conf)
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
            DataQueue.Clear();
        }
    }
}
