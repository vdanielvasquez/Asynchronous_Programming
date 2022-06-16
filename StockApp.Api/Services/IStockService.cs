using StockApp.Api.Models;

namespace StockApp.Api.Services;

public interface IStockService
{
    Task<IEnumerable<StockPrice>> GetStockPricesForId(string id);

    Task<IEnumerable<StockPrice>> GetStockPricesForIdList(string csvIdList);
    
    
}