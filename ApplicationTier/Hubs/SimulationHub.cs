using Microsoft.AspNetCore.SignalR;
using ModelEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationTier.Hubs
{
    public class SimulationHub : Hub
    {
        public async Task SendData(string user, DataToSend data)
        {
            await Clients.All.SendAsync("SendSimulationData", user, data);
        }
    }
}
