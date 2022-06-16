using StockApp.Api.Models;

namespace StockApp.Api.Services;

public interface IStockStreamService
{
    IAsyncEnumerable<StockPrice> GetAllStockPrices(CancellationToken ct = default);
}