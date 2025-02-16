using StockDashboardAPI.Models;
using System.Text.Json;

namespace StockDashboardAPI.Services
{
    public class StockDataFetcherService : BackgroundService
    {
        private readonly ILogger<StockDataFetcherService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _stockApiUrl = "https://www.alphavantage.co/query?function=TIME_SERIES_WEEKLY&symbol=IBM&apikey=demo";
        public StockDataFetcherService(ILogger<StockDataFetcherService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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

                    stockData.Timestamp = DateTime.Now;

                    // TODO: Save or broadcast this stockData
                    // (I will push it to clients using SignalR in the next section)

                    _logger.LogInformation($"Stock data fetched: {stockData.Symbol} - {stockData.Price}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching stock data");
                }

                // Wait for a period before the next fetch
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
