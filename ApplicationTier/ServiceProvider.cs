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
        public void SendData(object sender, EventArgs e);
        public void SendData2(object sender, EventArgs e);
        public void StartSimulation();
        public void StartSimulation2();
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
        Initialization ini;

        public SimulationWrapper()
        {
            Simulation = new Simulation();
            udpClient = new();
            Simulation.DataSending += () =>
            {
                var sendBytes = Simulation.DataToSend;
                udpClient.Send(sendBytes, sendBytes.Length, "localhost", 33333);
            };
            DataSendTimer = new System.Timers.Timer(125);
            DataSendTimer.Elapsed += SendData2;
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

        public void SendData(object sender, EventArgs e)
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
        }

        public void SendData2(object sender, EventArgs e)
        {
            //Console.WriteLine("senddara2 called");
            bool shouldStop;
            lock (Simulation)
            {
                shouldStop = !Simulation.Simulate50(DataQueue);
                Console.WriteLine(Simulation.Detect());           
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            }

            if (shouldStop)
            {
                Console.WriteLine("inside if2");
                DataSendTimer.Stop();
                Console.WriteLine("Timer Stoped");
                Console.WriteLine(Simulation.Step_count);
                Console.WriteLine(Simulation.Plane.Position.ToString("G40"));
                //SaveToDatabase(ini, Simulation.Configuration);
            }if (DataQueue.TryDequeue(out DataToSend data))
            {
                Console.WriteLine("tried dequeue");
                Connection.InvokeAsync("SendData", "user", data);
            }
        }

        public void StartSimulation2()
        {
            ini = new Initialization()
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
                Alpha = Simulation.Plane.Alpha,
                Vk = Simulation.Plane.Vk,
                XShip = Simulation.Ship.Position[0],
                YShip = Simulation.Ship.Position[1],
                ZShip = Simulation.Ship.Position[2],
                PsiShip = Simulation.Ship.Psi
            };
            DataSendTimer.Start();
            //Thread.Sleep(20000);
        }

        public void StartSimulation()
        {
            ini = new Initialization()
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
                Alpha = Simulation.Plane.Alpha,
                Vk = Simulation.Plane.Vk,
                XShip = Simulation.Ship.Position[0],
                YShip = Simulation.Ship.Position[1],
                ZShip = Simulation.Ship.Position[2],
                PsiShip = Simulation.Ship.Psi
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
            DataSendTimer.Stop();
        }
    }
}
