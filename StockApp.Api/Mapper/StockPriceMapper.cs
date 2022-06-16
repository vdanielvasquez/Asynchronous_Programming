using System.Globalization;
using StockApp.Api.Models;

namespace StockApp.Api.Mapper;

public static class StockPriceMapper
{
    public static StockPrice MapFromCsvLine(string line)
    {
        // Split the comma separated values
        var segments = line.Split(',');

        // Remove unnecessary characters and spaces
        for (var i = 0; i < segments.Length; i++) 
            segments[i] = segments[i].Trim('\'', '"');

        // Parse to a StockPrice instance
        var price = new StockPrice
        {
            Identifier = segments[0],
            TradeDate = DateTime.ParseExact(segments[1], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
            Volume = Convert.ToInt32(segments[6], CultureInfo.InvariantCulture),
            Change = Convert.ToDecimal(segments[7], CultureInfo.InvariantCulture),
            ChangePercent = Convert.ToDecimal(segments[8], CultureInfo.InvariantCulture),
        };

        return price;
    }
}