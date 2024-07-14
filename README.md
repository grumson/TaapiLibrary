# TaapiLibrary

The `TaapiLibrary` is a library that provides a client for interacting with the TAAPI API. It allows users to retrieve indicator values and perform bulk indicator requests.

## TaapiClient

The `TaapiClient` class is the main class in the `TaapiLibrary` that provides methods for interacting with the TAAPI API.

### Constructors

#### `TaapiClient(int retryAfterSeconds = 60)`

- Initializes a new instance of the `TaapiClient` class.
- Parameters:
  - `retryAfterSeconds` (optional): The number of seconds to wait before retrying a request if the rate limit is exceeded. The default value is 60 seconds.

### Public Methods

#### `Task<TaapiIndicatorValuesResponse> GetIndicatorAsync(string apiKey, string symbol, TaapiExchange exchange, TaapiCandlesInterval candlesInterval, TaapiIndicatorPropertiesRequest directParametersRequest)`

- Retrieves indicator values for a specific symbol and exchange.
- Parameters:
  - `apiKey`: The API key for accessing the TAAPI API.
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
  - A `Task` that represents the asynchronous operation. The task result contains a list of `TaapiBulkResponse` objects, each representing the response for an individual indicator request.

### Exceptions

The `TaapiClient` class may throw the following exceptions:

- `ArgumentException`: Thrown when the symbol, API key, or indicator request is null or empty.
- `UnauthorizedAccessException`: Thrown when the API key is invalid.
- `RateLimitExceededException`: Thrown when the rate limit is exceeded.
- `Exception`: Thrown when an error occurs while fetching the indicator.

## Usage

To use the `TaapiClient` class, you need to create an instance of it and call the appropriate methods with the required parameters. Here's an example:
Make sure to replace `apiKey`, `symbol`, `exchange`, `candlesInterval`, `directParametersRequest`, and `bulkRequest` with the actual values specific to your use case.

For more information on the TAAPI API and the available indicators, please refer to the TAAPI documentation.
