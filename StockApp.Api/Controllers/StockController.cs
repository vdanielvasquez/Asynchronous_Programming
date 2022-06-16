using Microsoft.AspNetCore.Mvc;
using StockApp.Api.Services;

namespace StockApp.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class StockController : ControllerBase
{
    private readonly IStockService _service;
    private readonly IStockStreamService _streamService;

    public StockController(IStockService service, IStockStreamService streamService)
    {
        _service = service;
        _streamService = streamService;
    }
    
    /// <summary>
    /// Gets the stock price for specific name
    /// </summary>
    /// <param name="name">name of the stock</param>
    /// <returns></returns>
    [Route("[controller]/{name}")]
    [HttpGet]
    public async Task<IActionResult> Get(string name)
    {
        var stocks = await _service.GetStockPricesForId(name);

        if (!stocks.Any())
            return NotFound("No stock prices");
        return Ok(stocks);
    }
    
    /// <summary>
    /// Gets the stock price for specific name
    /// </summary>
    /// <param name="csvNames">name of the stock in a comma separated value format</param>
    /// <returns></returns>
    [Route("[controller]/list/{csvNames}")]
    [HttpGet]
    public async Task<IActionResult> GetFromList(string csvNames)
    {
        var stocks = await _service.GetStockPricesForIdList(csvNames);

        if (!stocks.Any())
            return NotFound("No stock prices");
        return Ok(stocks);
    }
    
    /// <summary>
    /// Gets the stock price for specific name
    /// </summary>
    /// <param name="csvNames">name of the stock in a comma separated value format</param>
    /// <returns></returns>
    [Route("[controller]/stream/{csvNames}")]
    [HttpGet]
    public async Task<IActionResult> GetFromStream(string csvNames)
    {
        var stocks =  _streamService.GetAllStockPrices();

        if (stocks is null)
            return NotFound("No stock prices");
        return Ok(stocks);
    }
    
}