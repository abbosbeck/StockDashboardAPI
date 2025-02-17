using Microsoft.AspNetCore.Mvc;
using StockDashboardAPI.Models;
using System.Text.Json;

namespace StockDashboardAPI.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public StockController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet("volume")]
        public async Task<IActionResult> GetStockVolume()
        {
            string apiUrl = "https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=IBM&interval=1min&apikey=demo";
            string json = await _httpClient.GetStringAsync(apiUrl);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            StockData stockData = JsonSerializer.Deserialize<StockData>(json, options);

            if (stockData?.Valume != null && stockData.Valume.Count > 0)
            {
                var volumeData = stockData.Valume
                    .Select(entry => new { Timestamp = entry.Key, Volume = entry.Value.Volume })
                    .OrderBy(entry => entry.Timestamp) // Sort by time
                    .ToList();

                return Ok(volumeData);
            }

            return NotFound("No stock data found.");
        }
    }

}
