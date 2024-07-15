# TaapiLibrary Documentation

The TaapiLibrary is a C# library that provides a client for interacting with the Taapi API. It allows you to retrieve indicator values and perform bulk indicator requests.

## TaapiClient Class

The `TaapiClient` class is the main class in the TaapiLibrary. It provides methods for interacting with the Taapi API.

### Constructors

#### `TaapiClient(int retryAfterSeconds = 60)`

- Initializes a new instance of the `TaapiClient` class.
- Parameters:
  - `retryAfterSeconds` (optional): The number of seconds to wait before retrying a request if the rate limit is exceeded. Default value is 60 seconds.

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
using TaapiLibrary.Enums;


string _secret = "YOUR_API_KEY";

// Create a new Taapi Client
TaapiClient taapiClient = new TaapiClient();

// Get the indicator by calling the GetIndicatorAsync method
 var dataBnb = await taapiClient.GetIndicatorAsync(_secret, "BNB/USDT", TaapiExchange.Binance, TaapiCandlesInterval.OneMinute, 
                                                    new TaapiIndicatorPropertiesRequest(TaapiIndicatorType.SuperTrend, TaapiChart.Candles, period: 10, multiplier: 1));


 // Set Taapi bulk request

#region BNB/USDT
// Create a 3 new Taapi indicator requests
TaapiIndicatorPropertiesRequest indicatorRequest_BNB_1 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend, 
    TaapiChart.Candles,
    id: "BNB_superTrend_1",
    period: 10, 
    multiplier: 1
    );
TaapiIndicatorPropertiesRequest indicatorRequest_BNB_2 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "BNB_superTrend_2",
    period: 11,
    multiplier: 2
    );
TaapiIndicatorPropertiesRequest indicatorRequest_BNB_3 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "BNB_superTrend_3",
    period: 12,
    multiplier: 3
    );

List<TaapiIndicatorPropertiesRequest> indicators_BNB = new List<TaapiIndicatorPropertiesRequest> {
    indicatorRequest_BNB_1,
    indicatorRequest_BNB_2,
    indicatorRequest_BNB_3
};

#endregion

#region SOL/USDT

// Create a 3 new Taapi indicator requests
TaapiIndicatorPropertiesRequest indicatorRequest_SOL_1 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "SOL_superTrend_1",
    period: 10,
    multiplier: 1
    );
TaapiIndicatorPropertiesRequest indicatorRequest_SOL_2 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "SOL_superTrend_2",
    period: 11,
    multiplier: 2
    );
TaapiIndicatorPropertiesRequest indicatorRequest_SOL_3 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "SOL_superTrend_3",
    period: 12,
    multiplier: 3
    );

List<TaapiIndicatorPropertiesRequest> indicators_SOL = new List<TaapiIndicatorPropertiesRequest> {  
    indicatorRequest_SOL_1,
    indicatorRequest_SOL_2,
    indicatorRequest_SOL_3
};

#endregion

#region BTC/USDT

// Create a 3 new Taapi indicator requests
TaapiIndicatorPropertiesRequest indicatorRequest_BTC_1 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "BTC_superTrend_1",
    period: 10,
    multiplier: 1
    );
TaapiIndicatorPropertiesRequest indicatorRequest_BTC_2 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "BTC_superTrend_2",
    period: 11,
    multiplier: 2
    );
TaapiIndicatorPropertiesRequest indicatorRequest_BTC_3 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "BTC_superTrend_3",
    period: 12,
    multiplier: 3
    );

List<TaapiIndicatorPropertiesRequest> indicators_BTC = new List<TaapiIndicatorPropertiesRequest> {
    indicatorRequest_BTC_1,
    indicatorRequest_BTC_2,
    indicatorRequest_BTC_3
};

#endregion

#region ETH/USDT

// Create a 3 new Taapi indicator requests
TaapiIndicatorPropertiesRequest indicatorRequest_ETH_1 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "ETH_superTrend_1",
    period: 10,
    multiplier: 1
    );
TaapiIndicatorPropertiesRequest indicatorRequest_ETH_2 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "ETH_superTrend_2",
    period: 11,
    multiplier: 2
    );
TaapiIndicatorPropertiesRequest indicatorRequest_ETH_3 = new TaapiIndicatorPropertiesRequest(
    TaapiIndicatorType.SuperTrend,
    TaapiChart.Candles,
    id: "ETH_superTrend_3",
    period: 12,
    multiplier: 3
    );

List<TaapiIndicatorPropertiesRequest> indicators_ETH = new List<TaapiIndicatorPropertiesRequest> {
    indicatorRequest_ETH_1,
    indicatorRequest_ETH_2,
    indicatorRequest_ETH_3
};

#endregion


// Create a max 3 new Taapi bulk construct
TaapiBulkConstruct bulkConstruct_1 = new TaapiBulkConstruct(TaapiExchange.Binance, "BNB/USDT", TaapiCandlesInterval.OneMinute, indicators_BNB);
TaapiBulkConstruct bulkConstruct_2 = new TaapiBulkConstruct(TaapiExchange.Binance, "SOL/USDT", TaapiCandlesInterval.OneMinute, indicators_SOL);
TaapiBulkConstruct bulkConstruct_3 = new TaapiBulkConstruct(TaapiExchange.Binance, "BTC/USDT", TaapiCandlesInterval.OneMinute, indicators_BTC);

List<TaapiBulkConstruct> bulkConstructs = new List<TaapiBulkConstruct> {
    bulkConstruct_1,
    bulkConstruct_2,
    bulkConstruct_3
};


// Create a new Taapi bulk request
TaapiBulkRequest bulkRequest = new TaapiBulkRequest( _secret, bulkConstructs);



// Get the bulk indicators by calling the PostBulkIndicatorsAsync method
var bulkIndicators = taapiClient.PostBulkIndicatorsAsync(bulkRequest).Result;

```

This is just a basic example. You can customize the symbol, exchange, candles interval, and indicator properties according to your needs.

That's it! You now have an overview of the TaapiLibrary and how to use the `TaapiClient` class to interact with the Taapi API.
