namespace StockDashboardAPI.Models
{
    public class StockData
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
