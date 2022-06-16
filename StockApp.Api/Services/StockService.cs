using StockApp.Api.Mapper;
using StockApp.Api.Models;

namespace StockApp.Api.Services;

public class StockService : IStockService
{
    private readonly string _dataFilePath = "./Data/StockPrices_Small.csv";
    private CancellationTokenSource _cts;

    public StockService()
    {
        _cts = new CancellationTokenSource();
    }

    public async Task<IEnumerable<StockPrice>> GetStockPricesForId(string id)
    {
        if (!CheckCancellationToken()) return null;
        
        return await GetForId_Task_Continue(id, _cts.Token);
    }

    public async Task<IEnumerable<StockPrice>> GetStockPricesForIdList(string csvIdList)
    {
        if (!CheckCancellationToken()) return null;
        
        var loadingTasks = new List<Task<IEnumerable<StockPrice>>>();
        var ids = csvIdList.Split(',', ' ');
        foreach (var id in ids)
        {
            if (id.Length <= 0) continue;
            Console.WriteLine($"searching {id}");
            
            var loadTask = GetForId_Task_Continue(id, _cts.Token);
            loadingTasks.Add(loadTask);
        }

        var allStocks = await Task.WhenAll(loadingTasks);

        return allStocks.SelectMany(x => x); //flattens
    }

    private bool CheckCancellationToken()
    {
        if (_cts is {IsCancellationRequested: true})
        {
            _cts.Cancel();
            _cts = null;
            return false;
        }

        return true;
    }

    private async Task<IEnumerable<StockPrice>> GetForId_Task_Continue(string id, CancellationToken ct)
    {
        var linesTask = GetLinesFromCSV(ct);
        
        var stocksTask = linesTask.ContinueWith((completedTask) =>
            {
                var lines = completedTask.Result;

                var data = new List<StockPrice>();
                foreach (var line in lines.Skip(1)) //skips header
                {
                    var price = StockPriceMapper.MapFromCsvLine(line);
                    data.Add(price);
                }

                Console.WriteLine($"Stocks mapped: {data.Count}");
                return data;
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

        return await stocksTask.ContinueWith((completedTask) =>
        {
            var stocks = completedTask.Result;
            Console.Write("starts searching ");
            
            var found = stocks.Where(x => x.Identifier == id);
            Console.WriteLine($"found: {found.Count()}");
            return found;
        }, TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    private Task<List<string>> GetLinesFromCSV(CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            string line;
            var lines = new List<string>();
            using var stream = new StreamReader(File.OpenRead(_dataFilePath));
            while ((line = await stream.ReadLineAsync()) != null)
            {
                if (cancellationToken.IsCancellationRequested) break;
                lines.Add(line);
            }

            Console.WriteLine($"Files read from {_dataFilePath} count: {lines.Count}");
            return lines;
        }, cancellationToken);
    }
    
}