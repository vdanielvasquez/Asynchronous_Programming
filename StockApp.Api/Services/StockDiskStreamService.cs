using StockApp.Api.Mapper;
using StockApp.Api.Models;

namespace StockApp.Api.Services;

public class StockDiskStreamService : IStockStreamService
{
    private readonly string _dataFilePath = "./Data/StockPrices_Small.csv";
    
    public async IAsyncEnumerable<StockPrice> GetAllStockPrices(CancellationToken ct = default)
    {
        using var stream = new StreamReader(File.OpenRead(_dataFilePath));

        await stream.ReadLineAsync(); //skips the header row of the file 

        string line;
        int i = 0;
        while ((line = await stream.ReadLineAsync()) != null)
        {
            if (ct.IsCancellationRequested) break;
            i++;
            if (i == 9000) break;
            yield return StockPriceMapper.MapFromCsvLine(line);
        }
    }
}