using Microsoft.AspNetCore.SignalR;
using StockDashboardAPI.Models;

namespace StockDashboardAPI.Hubs
{
    public class StockHub : Hub
    {
        public async Task BroadcastStockData(StockData stockData)
        {
            await Clients.All.SendAsync("ReceiveStockData", stockData);
        }
    }
}
