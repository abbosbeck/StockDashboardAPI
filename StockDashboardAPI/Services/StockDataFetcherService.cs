using Microsoft.AspNetCore.SignalR;
using StockDashboardAPI.Hubs;
using StockDashboardAPI.Models;
using System.Text.Json;

namespace StockDashboardAPI.Services
{
    public class StockDataFetcherService : BackgroundService
    {
        private readonly ILogger<StockDataFetcherService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHubContext<StockHub> _hubContext;
        private readonly string _stockApiUrl = "https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=IBM&interval=1min&apikey=demo";
        public StockDataFetcherService(ILogger<StockDataFetcherService> logger, HttpClient httpClient, IHubContext<StockHub> hubContext)
        {
            _logger = logger;
            _httpClient = httpClient;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTime startingDate = new DateTime(2025, 02, 14, 16, 28, 0);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await _httpClient.GetAsync(_stockApiUrl, stoppingToken);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();

                    var stockData = JsonSerializer.Deserialize<StockData>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    //stockData.Timestamp = startingDate.AddSeconds(10);

                    await _hubContext.Clients.All.SendAsync("ReceiveStockData", stockData, cancellationToken: stoppingToken);

                    var latestEntry = stockData.Valume.OrderByDescending(x => x.Key).First();   

                    _logger.LogInformation($"Broadcasted stock data: {int.Parse(latestEntry.Value.Volume) - 8.7}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching or broadcasting stock data");
                }

                // Wait for a period before the next fetch
                await Task.Delay(TimeSpan.FromSeconds(40), stoppingToken);
            }
        }
    }
}
