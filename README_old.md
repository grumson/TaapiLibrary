

# TaapiLibrary Documentation

The TaapiLibrary is a C# library that provides a client for interacting with the Taapi API. It allows you to retrieve indicator values and perform bulk indicator requests.

Suported exchanges:
- Binance
- Binance Futures
- BinanceUs
- Coinbase
- Kraken
- Bitstamp
- WhiteBIT
- ByBit
- GateIo

Supported indicators:
- RSI
- macd
- sma
- ema
- stochastic
- bbands
- supertrend
- atr
- stochrsi
- ma
- dmi

## TaapiClient Class

The `TaapiClient` class is the main class in the TaapiLibrary. It provides methods for interacting with the Taapi API.

### Constructors

#### `TaapiClient(int retryAfterSeconds = 60)`

- Initializes a new instance of the `TaapiClient` class with optional parameters for the base URL and retry delay.
- Parameters:
  - `baseUrl` (optional): The base URL for the Taapi API. Default value is "https://api.taapi.io".
  - `retryAfterSeconds` (optional): The number of seconds to wait before retrying a request if the rate limit is exceeded. Default value is 60 seconds.

### Public
### Public Methods

#### `Task<TaapiIndicatorValuesResponse> GetIndicatorAsync(string apiKey, string symbol, TaapiExchange exchange, TaapiCandlesInterval candlesInterval, TaapiIndicatorPropertiesRequest directParametersRequest)`

- Retrieves indicator values for a specific symbol.
- Parameters:
  - `apiKey`: The API key for accessing the Taapi API.
  - `symbol`: The symbol for which to retrieve indicator values.
  - `exchange`: The exchange on which the symbol is traded.
  - `candlesInterval`: The interval of the candles for the indicator calculation.
  - `directParametersRequest`: The request object containing the indicator properties.
- Returns:
  - A `Task` that represents the asynchronous operation. The task result contains the indicator values as a `TaapiIndicatorValuesResponse` object.

#### `Task<List<TaapiBulkResponse>> PostBulkIndicatorsAsync(TaapiBulkRequest requests)`

- Performs bulk indicator requests.
- Parameters:
  - `requests`: The bulk request object containing multiple indicator requests.
- Returns:
  - A `Task` that represents the asynchronous operation. The task result contains a list of `TaapiBulkResponse` objects, each representing the result of an individual indicator request.

### Exception Handling

The `TaapiClient` class throws various exceptions to handle different error scenarios. These exceptions include:
- `ArgumentException`: Thrown when invalid arguments are passed to the methods.
- `UnauthorizedAccessException`: Thrown when the API key is invalid or unauthorized.
- `RateLimitExceededException`: Thrown when the rate limit is exceeded.
- `HttpRequestException`: Thrown when an HTTP request error occurs.
- `JsonException`: Thrown when a JSON serialization or deserialization error occurs.
- `InvalidOperationException`: Thrown when an invalid operation error occurs.
- `TaskCanceledException`: Thrown when a task is canceled.
- `UriFormatException`: Thrown when a URI format error occurs.
- `Exception`: Thrown for any other unexpected error.

## Example Usage

Here is an example of how to use the `TaapiClient` class to retrieve indicator values in console application:
Make sure to replace `"YOUR_API_KEY"` with your actual Taapi API key.

```csharp
using TaapiLibrary;
using TaapiLibrary.Contracts.Requests;
using TaapiLibrary.Contracts.Requests.Interfaces;
using TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;
using TaapiLibrary.Enums;
using TaapiLibrary.Models.Indicators.Properties;


Console.WriteLine("Taapi Test App");


string TAAPI_API_KEY = "your api key";

TaapiClient taapiClient = new TaapiClient();


// Create BNBUSDT list of indicators properties and add properties for each indicator
List<ITaapiIndicatorProperties> bnb_PropertiesList = new List<ITaapiIndicatorProperties>();
// Rsi propertie
RsiIndicatorPropertie rsi_bnb = new RsiIndicatorPropertie { 
    Id = "rsi_bnb",
    Chart = TaapiChart.Candles,
    Backtrack = 0,
    Period = 10,
};
bnb_PropertiesList.Add(rsi_bnb);
// SuperTrend propertie
SuperTrendIndicatorProperties superTrend_bnb = new SuperTrendIndicatorProperties {
    Id = "supertrend_bnb",
    Chart = TaapiChart.Candles,
    Backtrack = 1,
    Period = 10,
    Multiplier = 3,
};
bnb_PropertiesList.Add(superTrend_bnb);
// Stoch rsi propertie
StochRsiIndicatorProperties stochRsi_bnb = new StochRsiIndicatorProperties {
    Id = "stochrsi_bnb",
    Chart = TaapiChart.Candles,
    Backtrack = 0,
    ChartGaps = false,
    DPeriod = 10,
    KPeriod = 9,
    RsiPeriod = 11,
    StochasticPeriod = 12,
};
bnb_PropertiesList.Add(stochRsi_bnb);

// Create Binance Futures BNBUSDT Bulk Construct from indicators properties ( rsi, superTrend, stochRsi,... )
var bnb_Construct = taapiClient.CreateBulkConstruct(TaapiExchange.BinanceFutures, "BNB/USDT", TaapiCandlesInterval.OneMinute, bnb_PropertiesList);



// Create BTCUSDT list of indicators properties and add properties for each indicator
List<ITaapiIndicatorProperties> btc_PropertiesList = new List<ITaapiIndicatorProperties>();
// MACD propertie
MacdIndicatorPropertie macd_bnb = new MacdIndicatorPropertie {
    Id = "macd_bnb",
    Chart = TaapiChart.Heikinashi,
    Backtrack = 0,
    OptInFastPeriod = 10,
    OptInSignalPeriod = 11,
    OptInSlowPeriod = 12,
};
btc_PropertiesList.Add(macd_bnb);

// Create Binance Futures BTCUSDT 5 min candle interval Bulk Construct from indicators properties ( MACD,... )
var btc_Construct = taapiClient.CreateBulkConstruct(TaapiExchange.BinanceFutures, "BTC/USDT", TaapiCandlesInterval.FiveMinutes, btc_PropertiesList);



List<TaapiBulkConstruct> constructs = new List<TaapiBulkConstruct>();
constructs.Add(bnb_Construct);
constructs.Add(btc_Construct);


// Get Bulk request for all indicators ( max 3 Constructs )
var bulk = taapiClient.CreateBulkRequest(TAAPI_API_KEY, constructs);


// Get results for indicators
var results = await taapiClient.GetBulkIndicatorsResults(bulk);


// Print results
if(results?.Count > 0) {
    foreach (var result in results) {

        // Get symbol from Id
        string symbol = result.Id.Split("_")[1].ToUpper();

        // RSI
        if (result is IRsiIndicatorResults rsiResult) {
            Console.WriteLine($"Symbol: {symbol} - RSI: {rsiResult.Value}");
        }
        // SuperTrend
        else if (result is ISuperTrendIndicatorResults superTrendResult) {
            Console.WriteLine($"Symbol: {symbol} - SuperTrend: {superTrendResult.Value} - {superTrendResult.ValueAdvice}");
        }
        // Stoch RSI
        else if (result is IStochRsiIndicatorResults stochRsiResult) {
            Console.WriteLine($"Symbol: {symbol} - " +
                $"Stoch RSI FastK:{stochRsiResult.ValueFastK} - FastD:{stochRsiResult.ValueFastD}");
        }
        // MACD
        else if (result is IMacdIndicatorResults macdResult) {
            Console.WriteLine($"Symbol: {symbol} - MACD: {macdResult.ValueMACD} - Hist:{macdResult.ValueMACDHist} - Signal:{macdResult.ValueMACDSignal}");
        }


    }// foreach
}

Console.WriteLine("End of program");

```

# Changelog

This section outlines the changes and improvements made in each version of the TaapiLibrary.


## Version 1.0.3-alpha - 2023-07-18

### Added
- Posibility to set the `TaapiClient` base URL in the constructor.
- Support for additional exchanges in `TaapiExchange.cs`.

### Improved
- Performance optimizations in the `TaapiClient` class for faster API responses.

### Fixed
- Fixed an issue where `RateLimitExceededException` was not correctly handled in some scenarios.


## Version 1.0.2 - 2023-07-16

### Added
-

### Improved
- Some minor improvements.

### Fixed
- Some minor bug fixes and improvements.


## Version 1.0.1 - 2023-07-15

### Added
-

### Improved
- Some minor improvements.

### Fixed
- Some minor bug fixes and improvements.


## Version 1.0.0 - 2023-07-15

- Initial release of the TaapiLibrary.
- Support for basic indicator retrieval and bulk indicator requests.



This is just a basic example. You can customize the symbol, exchange, candles interval, and indicator properties according to your needs.

That's it! You now have an overview of the TaapiLibrary and how to use the `TaapiClient` class to interact with the Taapi API.
