using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using TaapiLibrary.Contracts.Requests;
using TaapiLibrary.Contracts.Requests.Interfaces;
using TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
using TaapiLibrary.Contracts.Response.Bulk;
using TaapiLibrary.Contracts.Response.Bulk.Interfaces;
using TaapiLibrary.Enums;
using TaapiLibrary.Exceptions;
using TaapiLibrary.Models.Indicators.Results;
using Microsoft.Extensions.Logging;

namespace TaapiLibrary;
public class TaapiClient {


    #region *** PROPERTIES ***

    /// <summary>
    /// HttpClient instance for making HTTP requests.
    /// </summary>
    private static readonly HttpClient _httpClient = new HttpClient();

    /// <summary>
    /// Base URL for the Taapi API.
    /// </summary>
    private readonly string _baseUrl;

    /// <summary>
    /// Number of seconds to wait before retrying a failed request.
    /// </summary>
    private readonly int _retryAfterSeconds;

    /// <summary>
    /// Logger instance for logging.
    /// </summary>
    private readonly ILogger<TaapiClient>? _logger;

    #endregion



    #region *** CONSTRUCTORS ***
    public TaapiClient(ILogger<TaapiClient>? logger = null, string baseUrl = "https://api.taapi.io", int retryAfterSeconds = 60) {

        _logger = logger;
        _baseUrl = baseUrl;
        _retryAfterSeconds = retryAfterSeconds;
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

    }
    #endregion



    #region *** PUBLIC METHODS ***

    /// <summary>
    /// Fetches indicator values asynchronously.
    /// </summary>
    /// <param name="apiKey">API key for authentication.</param>
    /// <param name="symbol">Symbol for which to fetch the indicator.</param>
    /// <param name="exchange">Exchange on which the symbol is traded.</param>
    /// <param name="candlesInterval">Interval for the candles.</param>
    /// <param name="directParametersRequest">Parameters for the indicator request.</param>
    /// <returns>Indicator values response.</returns>
    public async Task<TaapiIndicatorValuesResponse> GetIndicatorAsync(string apiKey, string symbol, TaapiExchange exchange, TaapiCandlesInterval candlesInterval, TaapiIndicatorPropertiesRequest directParametersRequest ) {

        ValidateParameters(apiKey, symbol, directParametersRequest);

        var parametersMandatory = $"exchange={exchange.GetDescription()}&symbol={symbol}&interval={candlesInterval.GetDescription()}";
        var parametersOptional = $"&backtrack={directParametersRequest.Backtrack}&gaps={directParametersRequest.Gaps}&period={directParametersRequest.Period}&stddev={directParametersRequest.StdDev}&multiplier={directParametersRequest.Multiplier}&optInFastPeriod={directParametersRequest.OptInFastPeriod}&optInSlowPeriod={directParametersRequest.OptInSlowPeriod}&optInSignalPeriod={directParametersRequest.OptInSignalPeriod}&kPeriod={directParametersRequest.KPeriod}&dPeriod={directParametersRequest.DPeriod}";
        var url = $"{_baseUrl}/{directParametersRequest.Indicator}?secret={apiKey}&{parametersMandatory}&{parametersOptional}";

        try {

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
            await HandleHttpErrors(response).ConfigureAwait(false);

            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<TaapiIndicatorValuesResponse>(jsonString) ?? new TaapiIndicatorValuesResponse();
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Unexpected error occurred while fetching indicator.");
            throw;
        }

        /*try {

            // Send the request
            var response = await _httpClient.GetAsync(url);

            // Unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized) {
                Console.WriteLine("Unauthorized. Invalid API key.");
                throw new UnauthorizedAccessException("Unauthorized. Invalid API key.");
            }
            // Rate limit exceeded
            else if (response.StatusCode == HttpStatusCode.TooManyRequests) {
                // Obravnava presežene omejitve hitrosti
                var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? 60; // Predpostavimo 60 sekund, če Retry-After glava ni podana
                Console.WriteLine($"Rate limit exceeded. Retry after {retryAfter} seconds.");
                throw new RateLimitExceededException($"Rate limit exceeded. Retry after {retryAfter} seconds.", retryAfter);
            }


            if (!response.IsSuccessStatusCode) {

                // Logiranje in obdelava različnih statusnih kod
                var errorContent = await response.Content.ReadAsStringAsync();
                // Log the error to the console
                Console.WriteLine($"Error fetching indicator: {response.StatusCode}, {errorContent}");
                // Throw an exception for not correct parameters
                throw new Exception($"Error fetching indicator: {response.StatusCode}, {errorContent}");
            }


            var jsonString = await response.Content.ReadAsStringAsync();

            // Deserialize the response
            TaapiIndicatorValuesResponse? taapiIndicatorValuesResponse = JsonConvert.DeserializeObject<TaapiIndicatorValuesResponse>(jsonString);

            if (taapiIndicatorValuesResponse != null) {
                return taapiIndicatorValuesResponse;
            }

        }
        catch (HttpRequestException e) {
            Console.WriteLine($"Request error: {e.Message}");
            throw;
        }
        catch (JsonException e) {
            Console.WriteLine($"JSON deserialization error: {e.Message}");
            throw;
        }
        catch (InvalidOperationException invalidOperationException) {
            // Handle invalid operation exceptions
            Console.WriteLine($"Invalid Operation Error: {invalidOperationException.Message}");
            throw;
        }
        catch (TaskCanceledException taskCanceledException) {
            // Handle task canceled exceptions
            Console.WriteLine($"Task Canceled Error: {taskCanceledException.Message}");
            throw;
        }
        catch (UriFormatException UriFormatException) {
            // Handle URI format exceptions
            Console.WriteLine($"URI Format Error: {UriFormatException.Message}");
            throw;
        }
        catch (Exception e) {
            Console.WriteLine($"Unexpected error: {e.Message}");
            throw;
        }
        

        return new TaapiIndicatorValuesResponse();
        */
    }//end GetIndicatorAsync()


    // <summary>
    /// Posts bulk indicators asynchronously. This method is deprecated.
    /// </summary>
    /// <param name="requests">Bulk request containing multiple indicator requests.</param>
    /// <returns>List of bulk responses.</returns>
    [Obsolete("GetIndicatorAsync is deprecated, please use the new method GetBulkIndicatorsResults instead.", true)]
    public async Task<List<TaapiBulkResponse>> PostBulkIndicatorsAsync(TaapiBulkRequest requests) {

        // Check if the requests are null
        if (requests == null) {
            throw new ArgumentNullException(nameof(requests), "The requests cannot be null.");
        }

        // Set the URL
        var url = $"{_baseUrl}/bulk";

        // Serialize the request
        var content = new StringContent(JsonConvert.SerializeObject(requests), Encoding.UTF8, "application/json");

        try {

            // Send the request
            var response = await _httpClient.PostAsync(url, content);

            // Check the response status code

            // Unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized) {
                throw new UnauthorizedAccessException("Unauthorized. Invalid API key.");
            }
            // Rate limit exceeded
            else if (response.StatusCode == HttpStatusCode.TooManyRequests) {
                // Obravnava presežene omejitve hitrosti
                var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? _retryAfterSeconds; // Predpostavimo 60 sekund, če Retry-After glava ni podana
                throw new RateLimitExceededException($"Rate limit exceeded. Retry after {retryAfter} seconds.", retryAfter);
            }

            // Check if the response is successful
            response.EnsureSuccessStatusCode();

            // Read the response content
            var jsonString = await response.Content.ReadAsStringAsync();

            // Chek if the response have errors


            // Deserialize the response
            TaapiBulkResponse? taapiBulkResponse = JsonConvert.DeserializeObject<TaapiBulkResponse>(jsonString);

            // Return the response
            if (taapiBulkResponse != null) {
                return new List<TaapiBulkResponse> { taapiBulkResponse };
            }
            // Return an empty list if the response is null
            else {
                return new List<TaapiBulkResponse>();
            }

        }
        catch (HttpRequestException httpRequestException) {
            // Handle HTTP request specific exceptions
            Console.WriteLine($"HTTP Request Error: {httpRequestException.Message}");
            throw;
        }
        catch (JsonException jsonException) {
            // Handle JSON serialization/deserialization exceptions
            Console.WriteLine($"JSON Error: {jsonException.Message}");
            throw;
        }
        catch (InvalidOperationException invalidOperationException) {
            // Handle invalid operation exceptions
            Console.WriteLine($"Invalid Operation Error: {invalidOperationException.Message}");
            throw;
        }
        catch (TaskCanceledException taskCanceledException) { 
            // Handle task canceled exceptions
            Console.WriteLine($"Task Canceled Error: {taskCanceledException.Message}");
            throw;
        }
        catch (UriFormatException UriFormatException) { 
            // Handle URI format exceptions
            Console.WriteLine($"URI Format Error: {UriFormatException.Message}");
            throw;
        }
        catch (Exception ex) {
            // Handle any other exceptions
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }


    }//end PostBulkIndicatorsAsync()


    /// <summary>
    /// Fetches bulk indicator results asynchronously.
    /// </summary>
    /// <param name="requests">Bulk request containing multiple indicator requests.</param>
    /// <returns>List of indicator results.</returns>
    public async Task<List<ITaapiIndicatorResults>> GetBulkIndicatorsResults(TaapiBulkRequest requests) {

        ValidateBulkRequest(requests);

        var url = $"{_baseUrl}/bulk";
        var content = new StringContent(JsonConvert.SerializeObject(requests), Encoding.UTF8, "application/json");

        try {
            var response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
            await HandleHttpErrors(response).ConfigureAwait(false);

            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var taapiBulkResponse = JsonConvert.DeserializeObject<TaapiBulkResponse>(jsonString);
            return taapiBulkResponse != null ? GetIndicatorResults(taapiBulkResponse) : new List<ITaapiIndicatorResults>();
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Unexpected error occurred while fetching bulk indicator results.");
            throw;
        }

        /*
        // Check if the requests are null
        if (requests == null) {
            throw new ArgumentNullException(nameof(requests), "The requests cannot be null.");
        }

        // Set the URL
        var url = $"{_baseUrl}/bulk";

        // Serialize the request
        var content = new StringContent(JsonConvert.SerializeObject(requests), Encoding.UTF8, "application/json");

        try {

            // Send the request
            var response = await _httpClient.PostAsync(url, content);

            // Check the response status code

            // Unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized) {
                throw new UnauthorizedAccessException("Unauthorized. Invalid API key.");
            }
            // Rate limit exceeded
            else if (response.StatusCode == HttpStatusCode.TooManyRequests) {
                // Obravnava presežene omejitve hitrosti
                var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? _retryAfterSeconds; // Predpostavimo 60 sekund, če Retry-After glava ni podana
                throw new RateLimitExceededException($"Rate limit exceeded. Retry after {retryAfter} seconds.", retryAfter);
            }

            // Check if the response is successful
            response.EnsureSuccessStatusCode();

            // Read the response content
            var jsonString = await response.Content.ReadAsStringAsync();

            // Chek if the response have errors

            // Deserialize the response
            TaapiBulkResponse? taapiBulkResponse = JsonConvert.DeserializeObject<TaapiBulkResponse>(jsonString);

            //// Get the indicators results from the response
            List<ITaapiIndicatorResults> taapiIndicatorResultsList = GetIndicatorResults(taapiBulkResponse!);

            // Return the indicators results
            return taapiIndicatorResultsList;
        }
        catch (HttpRequestException httpRequestException) {
            // Handle HTTP request specific exceptions
            Console.WriteLine($"HTTP Request Error: {httpRequestException.Message}");
            throw;
        }
        catch (JsonException jsonException) {
            // Handle JSON serialization/deserialization exceptions
            Console.WriteLine($"JSON Error: {jsonException.Message}");
            throw;
        }
        catch (InvalidOperationException invalidOperationException) {
            // Handle invalid operation exceptions
            Console.WriteLine($"Invalid Operation Error: {invalidOperationException.Message}");
            throw;
        }
        catch (TaskCanceledException taskCanceledException) {
            // Handle task canceled exceptions
            Console.WriteLine($"Task Canceled Error: {taskCanceledException.Message}");
            throw;
        }
        catch (UriFormatException UriFormatException) {
            // Handle URI format exceptions
            Console.WriteLine($"URI Format Error: {UriFormatException.Message}");
            throw;
        }
        catch (Exception ex) {
            // Handle any other exceptions
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
        */

    }//end GetBulkIndicatorsResults()


    /// <summary>
    /// Creates a bulk request for multiple indicators.
    /// </summary>
    /// <param name="apiKey">API key for authentication.</param>
    /// <param name="bulkConstructList">List of bulk constructs.</param>
    /// <returns>Bulk request object.</returns>
    public TaapiBulkRequest CreateBulkRequest(string apiKey, List<TaapiBulkConstruct> bulkConstructList) {

        ValidateApiKeyAndConstructList(apiKey, bulkConstructList);
        return new TaapiBulkRequest(apiKey, bulkConstructList);

        /*

        #region Validate

        // check if the apiKey is null or empty
        if (string.IsNullOrWhiteSpace(apiKey)) {
            throw new ArgumentException("The API key cannot be null or empty.");
        }

        // check if the bulkConstructList is null or empty
        if (bulkConstructList == null || bulkConstructList.Count == 0) {
            throw new ArgumentException("The list of bulk constructs cannot be null or empty.");
        }

        #endregion


        TaapiBulkRequest bulkRequest = new TaapiBulkRequest(apiKey, bulkConstructList);


        return bulkRequest;
        */

    }//end CreateBulkRequest()


    /// <summary>
    /// Creates a bulk construct for a specific exchange, symbol, and interval.
    /// </summary>
    /// <param name="exchange">Exchange on which the symbol is traded.</param>
    /// <param name="symbol">Symbol for which to create the construct.</param>
    /// <param name="candlesInterval">Interval for the candles.</param>
    /// <param name="indicatorList">List of indicators to include in the construct.</param>
    /// <returns>Bulk construct object.</returns>
    public TaapiBulkConstruct CreateBulkConstruct(TaapiExchange exchange, string symbol, TaapiCandlesInterval candlesInterval, List<ITaapiIndicatorProperties> indicatorList) {

        ValidateSymbolAndIndicatorList(symbol, indicatorList);

        var taapiIndicatorPropertiesRequestList = indicatorList.Select(MapIndicatorRequest).ToList();
        return new TaapiBulkConstruct(exchange, symbol, candlesInterval, taapiIndicatorPropertiesRequestList);

        /*
        #region Validate

        // check if the symbol is null or empty
        if (string.IsNullOrWhiteSpace(symbol)) {
            throw new ArgumentException("The symbol cannot be null or empty.");
        }

        // check if the indicatorList is null or empty
        if (indicatorList == null || indicatorList.Count == 0) {
            throw new ArgumentException("The list of indicators cannot be null or empty.");
        }

        #endregion

        List<TaapiIndicatorPropertiesRequest> taapiIndicatorPropertiesRequestList = new List<TaapiIndicatorPropertiesRequest>();

        foreach (var indicatorRequest in indicatorList) {

            TaapiIndicatorPropertiesRequest taapiIndicatorPropertiesRequest = MapIndicatorRequest(indicatorRequest);

            taapiIndicatorPropertiesRequestList.Add(taapiIndicatorPropertiesRequest);
        }

        TaapiBulkConstruct bulkConstruct = new TaapiBulkConstruct(exchange, symbol, candlesInterval, taapiIndicatorPropertiesRequestList);

        return bulkConstruct;
        */
    }//end CreateBulkConstruct()

    #endregion



    #region *** PRIVATE METHODS ***

    /// <summary>
    /// Maps an indicator request to a TaapiIndicatorPropertiesRequest object.
    /// </summary>
    /// <param name="indicatorRequest">Indicator request to map.</param>
    /// <returns>Mapped TaapiIndicatorPropertiesRequest object.</returns>
    private TaapiIndicatorPropertiesRequest MapIndicatorRequest(ITaapiIndicatorProperties indicatorRequest) {

        var taapiIndicatorPropertiesRequest = indicatorRequest switch {
            IRsiIndicatorProperties rsi => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { Period = rsi.Period },
            IMacdIndicatorProperties macd => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { OptInFastPeriod = macd.OptInFastPeriod, OptInSlowPeriod = macd.OptInSlowPeriod, OptInSignalPeriod = macd.OptInSignalPeriod },
            ISmaIndicatorProperties sma => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { Period = sma.Period },
            IEmaIndicatorProperties ema => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { Period = ema.Period },
            IStochIndicatorProperties stoch => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { KPeriod = stoch.KPeriod, DPeriod = stoch.DPeriod, KSmooth = stoch.KSmooth },
            IBbandsIndicatorProperties bbands => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { Period = bbands.Period, StdDev = bbands.Stddev },
            ISuperTrendIndicatorProperties superTrend => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { Period = superTrend.Period, Multiplier = superTrend.Multiplier },
            IAtrIndicatorProperties atr => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { Period = atr.Period },
            IStochRsiIndicatorProperties stochRsi => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { KPeriod = stochRsi.KPeriod, DPeriod = stochRsi.DPeriod, RsiPeriod = stochRsi.RsiPeriod, StochasticPeriod = stochRsi.StochasticPeriod },
            IMaIndicatorProperties ma => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { Period = ma.Period },
            IDmiIndicatorProperties dmi => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { Period = dmi.Period },
            ICandleIndicatorProperties candle => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart),
            ICandlesIndicatorProperties candles => new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { Period = candles.Period },
            _ => throw new NotImplementedException("The indicator is not implemented.")
        };

        taapiIndicatorPropertiesRequest.Id = indicatorRequest.Id;
        taapiIndicatorPropertiesRequest.Backtrack = indicatorRequest.Backtrack;
        taapiIndicatorPropertiesRequest.AddResultTimestamp = indicatorRequest.AddResultTimestamp;
        taapiIndicatorPropertiesRequest.FromTimestamp = indicatorRequest.FromTimestamp;
        taapiIndicatorPropertiesRequest.ToTimestamp = indicatorRequest.ToTimestamp;
        taapiIndicatorPropertiesRequest.Results = indicatorRequest.Results;
        taapiIndicatorPropertiesRequest.Gaps = indicatorRequest.ChartGaps;

        return taapiIndicatorPropertiesRequest;

        /*
        TaapiIndicatorPropertiesRequest taapiIndicatorPropertiesRequest = null!;
            
        // RSI
        if (indicatorRequest is IRsiIndicatorProperties rsiIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                
                Period = rsiIndicatorProperties.Period,
            };
        }
        // MACD
        else if (indicatorRequest is IMacdIndicatorProperties macdIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                OptInFastPeriod = macdIndicatorProperties.OptInFastPeriod,
                OptInSlowPeriod = macdIndicatorProperties.OptInSlowPeriod,
                OptInSignalPeriod = macdIndicatorProperties.OptInSignalPeriod,
            };
        }
        // SMA
        else if (indicatorRequest is ISmaIndicatorProperties smaIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                Period = smaIndicatorProperties.Period,
            };
        }
        // EMA
        else if (indicatorRequest is IEmaIndicatorProperties emaIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                Period = emaIndicatorProperties.Period,
            };
        }
        // STOCH
        else if (indicatorRequest is IStochIndicatorProperties stochasticIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                KPeriod = stochasticIndicatorProperties.KPeriod,
                DPeriod = stochasticIndicatorProperties.DPeriod,
                KSmooth = stochasticIndicatorProperties.KSmooth,
            };
        }
        // BBANDS
        else if (indicatorRequest is IBbandsIndicatorProperties bbandsIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                Period = bbandsIndicatorProperties.Period,
                StdDev = bbandsIndicatorProperties.Stddev,
            };
        }
        // SUPER TREND
        else if (indicatorRequest is ISuperTrendIndicatorProperties superTrendIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                Period = superTrendIndicatorProperties.Period,
                Multiplier = superTrendIndicatorProperties.Multiplier,
            };
        }
        // ATR
        else if (indicatorRequest is IAtrIndicatorProperties atrIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                Period = atrIndicatorProperties.Period,
            };
        }
        // STOCH RSI
        else if (indicatorRequest is IStochRsiIndicatorProperties stochRsiIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                KPeriod = stochRsiIndicatorProperties.KPeriod,
                DPeriod = stochRsiIndicatorProperties.DPeriod,
                RsiPeriod = stochRsiIndicatorProperties.RsiPeriod,
                StochasticPeriod = stochRsiIndicatorProperties.StochasticPeriod,
            };
        }
        // MA
        else if (indicatorRequest is IMaIndicatorProperties maIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                Period = maIndicatorProperties.Period,
            };
        }
        // DMI
        else if (indicatorRequest is IDmiIndicatorProperties dmiIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                Period = dmiIndicatorProperties.Period,
            };
        }
        // CANDLE
        else if (indicatorRequest is ICandleIndicatorProperties candleIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) { };
        }
        // CANDLES
        else if (indicatorRequest is ICandlesIndicatorProperties candlesIndicatorProperties) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                Period = candlesIndicatorProperties.Period,
            };
        }
        // Not implemented
        else {

            throw new NotImplementedException("The indicator is not implemented.");
        }

        taapiIndicatorPropertiesRequest.Id = indicatorRequest.Id;
        taapiIndicatorPropertiesRequest.Backtrack = indicatorRequest.Backtrack;
        taapiIndicatorPropertiesRequest.AddResultTimestamp = indicatorRequest.AddResultTimestamp;
        taapiIndicatorPropertiesRequest.FromTimestamp = indicatorRequest.FromTimestamp;
        taapiIndicatorPropertiesRequest.ToTimestamp = indicatorRequest.ToTimestamp;
        taapiIndicatorPropertiesRequest.Results = indicatorRequest.Results;
        taapiIndicatorPropertiesRequest.Gaps = indicatorRequest.ChartGaps;

        return taapiIndicatorPropertiesRequest;
        */
    }//end MapIndicatorRequest()


    /// <summary>
    /// Maps a bulk data response to an indicator results object.
    /// </summary>
    /// <param name="taapiBulkDataResponse">Bulk data response to map.</param>
    /// <returns>Mapped indicator results object.</returns>
    private ITaapiIndicatorResults MapIndicatorResults(TaapiBulkDataResponse taapiBulkDataResponse) {

        return taapiBulkDataResponse.indicator switch {
            var indicator when indicator == TaapiIndicatorType.RSI.GetDescription() => new RsiIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, Value = taapiBulkDataResponse.result.value },
            var indicator when indicator == TaapiIndicatorType.MACD.GetDescription() => new MacdIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, ValueMACD = taapiBulkDataResponse.result.valueMACD, ValueMACDSignal = taapiBulkDataResponse.result.valueMACDSignal, ValueMACDHist = taapiBulkDataResponse.result.valueMACDHist },
            var indicator when indicator == TaapiIndicatorType.SMA.GetDescription() => new SmaIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, Value = taapiBulkDataResponse.result.value },
            var indicator when indicator == TaapiIndicatorType.EMA.GetDescription() => new EmaIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, Value = taapiBulkDataResponse.result.value },
            var indicator when indicator == TaapiIndicatorType.Stochastic.GetDescription() => new StochIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, ValueK = taapiBulkDataResponse.result.valueK, ValueD = taapiBulkDataResponse.result.valueD },
            var indicator when indicator == TaapiIndicatorType.BBands.GetDescription() => new BbandsIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, ValueUpperBand = taapiBulkDataResponse.result.valueUpperBand, ValueMiddleBand = taapiBulkDataResponse.result.valueMiddleBand, ValueLowerBand = taapiBulkDataResponse.result.valueLowerBand },
            var indicator when indicator == TaapiIndicatorType.SuperTrend.GetDescription() => new SuperTrendIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, Value = taapiBulkDataResponse.result.value, ValueAdvice = taapiBulkDataResponse.result.valueAdvice },
            var indicator when indicator == TaapiIndicatorType.Atr.GetDescription() => new AtrIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, Value = taapiBulkDataResponse.result.value },
            var indicator when indicator == TaapiIndicatorType.StochRsi.GetDescription() => new StochRsiIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, ValueFastK = taapiBulkDataResponse.result.valueFastK, ValueFastD = taapiBulkDataResponse.result.valueFastD },
            var indicator when indicator == TaapiIndicatorType.Ma.GetDescription() => new MaIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, Value = taapiBulkDataResponse.result.value },
            var indicator when indicator == TaapiIndicatorType.Dmi.GetDescription() => new DmiIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, Adx = taapiBulkDataResponse.result.Adx, Pdi = taapiBulkDataResponse.result.Pdi, Mdi = taapiBulkDataResponse.result.Mdi },
            var indicator when indicator == TaapiIndicatorType.Candle.GetDescription() => new CandleIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, Close = taapiBulkDataResponse.result.close, High = taapiBulkDataResponse.result.high, Low = taapiBulkDataResponse.result.low, Open = taapiBulkDataResponse.result.open, Timestamp = taapiBulkDataResponse.result.timestamp, TimestampHuman = taapiBulkDataResponse.result.timestampHuman, Volume = taapiBulkDataResponse.result.volume },
            var indicator when indicator == TaapiIndicatorType.Candles.GetDescription() => new CandlesIndicatorResults { Id = taapiBulkDataResponse.id, Indicator = taapiBulkDataResponse.indicator, Errors = taapiBulkDataResponse.errors, Candles = taapiBulkDataResponse.result.veluesList?.Select(candle => new CandleIndicatorResults { Close = candle.close, High = candle.high, Low = candle.low, Open = candle.open, Timestamp = candle.timestamp, Volume = candle.volume }).ToList() ?? new List<CandleIndicatorResults>() },
            _ => throw new NotImplementedException("The indicator is not implemented.")
        };

        /*
        ITaapiIndicatorResults taapiIndicatorResults = null!;

        // RSI
        if (taapiBulkDataResponse.indicator == TaapiIndicatorType.RSI.GetDescription()) {

            taapiIndicatorResults = new RsiIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                Value = taapiBulkDataResponse.result.value,
            };
        }
        // MACD
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.MACD.GetDescription()) {

            taapiIndicatorResults = new MacdIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                ValueMACD = taapiBulkDataResponse.result.valueMACD,
                ValueMACDSignal = taapiBulkDataResponse.result.valueMACDSignal,
                ValueMACDHist = taapiBulkDataResponse.result.valueMACDHist,
            };
        }
        // SMA
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.SMA.GetDescription()) {

            taapiIndicatorResults = new SmaIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                Value = taapiBulkDataResponse.result.value,
            };
        }
        // EMA
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.EMA.GetDescription()) {

            taapiIndicatorResults = new EmaIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                Value = taapiBulkDataResponse.result.value,
            };
        }
        // STOCH
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.Stochastic.GetDescription()) {

            taapiIndicatorResults = new StochIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                ValueK = taapiBulkDataResponse.result.valueK,
                ValueD = taapiBulkDataResponse.result.valueD,
            };
        }
        // BBANDS
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.BBands.GetDescription()) {

            taapiIndicatorResults = new BbandsIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                ValueUpperBand = taapiBulkDataResponse.result.valueUpperBand,
                ValueMiddleBand = taapiBulkDataResponse.result.valueMiddleBand,
                ValueLowerBand = taapiBulkDataResponse.result.valueLowerBand,
            };
        }
        // SUPER TREND
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.SuperTrend.GetDescription()) {

            taapiIndicatorResults = new SuperTrendIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                Value = taapiBulkDataResponse.result.value,
                ValueAdvice = taapiBulkDataResponse.result.valueAdvice,
            };
        }
        // ATR
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.Atr.GetDescription()) {

            taapiIndicatorResults = new AtrIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                Value = taapiBulkDataResponse.result.value,
            };
        }
        // STOCH RSI
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.StochRsi.GetDescription()) {

            taapiIndicatorResults = new StochRsiIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                ValueFastK = taapiBulkDataResponse.result.valueFastK,
                ValueFastD = taapiBulkDataResponse.result.valueFastD,
            };
        }
        // MA
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.Ma.GetDescription()) {

            taapiIndicatorResults = new MaIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                Value = taapiBulkDataResponse.result.value,
            };
        }
        // DMI
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.Dmi.GetDescription()) {

            taapiIndicatorResults = new DmiIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                Adx = taapiBulkDataResponse.result.Adx,
                Pdi = taapiBulkDataResponse.result.Pdi,
                Mdi = taapiBulkDataResponse.result.Mdi,
            };
        }
        // CANDLE
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.Candle.GetDescription()) {

            taapiIndicatorResults = new CandleIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                Close = taapiBulkDataResponse.result.close,
                High = taapiBulkDataResponse.result.high,
                Low = taapiBulkDataResponse.result.low,
                Open = taapiBulkDataResponse.result.open,
                Timestamp = taapiBulkDataResponse.result.timestamp,
                TimestampHuman = taapiBulkDataResponse.result.timestampHuman,
                Volume = taapiBulkDataResponse.result.volume,
            };
        }
        // CANDLES
        else if (taapiBulkDataResponse.indicator == TaapiIndicatorType.Candles.GetDescription()) {

            taapiIndicatorResults = new CandlesIndicatorResults {
                Id = taapiBulkDataResponse.id,
                Indicator = taapiBulkDataResponse.indicator,
                Errors = taapiBulkDataResponse.errors,
                Candles = new List<CandleIndicatorResults>(),
            };

            if (taapiBulkDataResponse.result.veluesList?.Count > 0) {
                foreach (var candle in taapiBulkDataResponse.result.veluesList) {
                    CandleIndicatorResults candleIndicatorResults = new CandleIndicatorResults {
                        Close = candle.close,
                        High = candle.high,
                        Low = candle.low,
                        Open = candle.open,
                        Timestamp = candle.timestamp,
                        Volume = candle.volume,
                    };
                    ((CandlesIndicatorResults)taapiIndicatorResults).Candles.Add(candleIndicatorResults);
                }
            }
        }
        // Not implemented
        else {

            throw new NotImplementedException("The indicator is not implemented.");
        }

        return taapiIndicatorResults;
        */

    }//end MapIndicatorResults()


    /// <summary>
    /// Extracts indicator results from a bulk response.
    /// </summary>
    /// <param name="taapiBulkResponse">Bulk response containing indicator data.</param>
    /// <returns>List of indicator results.</returns>
    private List<ITaapiIndicatorResults> GetIndicatorResults(TaapiBulkResponse taapiBulkResponse) {

        List<ITaapiIndicatorResults> taapiIndicatorResultsList = new List<ITaapiIndicatorResults>();

        if (taapiBulkResponse?.data?.Count > 0) {
            foreach (var taapiBulkDataResponseRow in taapiBulkResponse.data) {

                TaapiBulkDataResponse taapiBulkDataResponse = new TaapiBulkDataResponse {
                    id = taapiBulkDataResponseRow.id,
                    indicator = taapiBulkDataResponseRow.indicator,
                    errors = taapiBulkDataResponseRow.errors,
                };

                if (taapiBulkDataResponseRow.result is JArray jArrayResult) {
                    taapiBulkDataResponse.result = new TaapiIndicatorValuesResponse();
                    taapiBulkDataResponse.result.veluesList = jArrayResult.ToObject<List<TaapiIndicatorValuesResponse>>();
                }
                else if (taapiBulkDataResponseRow.result is JObject jObjectResult) {
                    taapiBulkDataResponse.result = jObjectResult.ToObject<TaapiIndicatorValuesResponse>() ?? new TaapiIndicatorValuesResponse();
                }
                else {
                    _logger?.LogWarning("The result is not a valid object.");
                }

                ITaapiIndicatorResults taapiIndicatorResults = MapIndicatorResults(taapiBulkDataResponse);

                taapiIndicatorResultsList.Add(taapiIndicatorResults);
            }
        }

        return taapiIndicatorResultsList;
    }//end GetIndicatorResults()


    /// <summary>
    /// Validates the parameters for fetching indicator values.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="symbol"></param>
    /// <param name="directParametersRequest"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ValidateParameters(string apiKey, string symbol, TaapiIndicatorPropertiesRequest directParametersRequest) {

        if (string.IsNullOrEmpty(symbol))
            throw new ArgumentException("The symbol cannot be null or empty.");
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentException("The API key cannot be null or empty.");
        if (directParametersRequest == null)
            throw new ArgumentException("The TaapiIndicatorPropertiesRequest cannot be null or empty.");

    }//end ValidateParameters()


    /// <summary>
    /// Validates the bulk request.
    /// </summary>
    /// <param name="requests"></param>
    /// <exception cref="ArgumentNullException"></exception>
    private void ValidateBulkRequest(TaapiBulkRequest requests) {

        if (requests == null)
            throw new ArgumentNullException(nameof(requests), "The requests cannot be null.");

    }//end ValidateBulkRequest()


    /// <summary>
    /// Validates the API key and the list of bulk constructs.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="bulkConstructList"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ValidateApiKeyAndConstructList(string apiKey, List<TaapiBulkConstruct> bulkConstructList) {

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("The API key cannot be null or empty.");
        if (bulkConstructList == null || bulkConstructList.Count == 0)
            throw new ArgumentException("The list of bulk constructs cannot be null or empty.");

    }//end ValidateApiKeyAndConstructList()


    /// <summary>
    /// Validates the symbol and the list of indicators.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="indicatorList"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ValidateSymbolAndIndicatorList(string symbol, List<ITaapiIndicatorProperties> indicatorList) {

        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("The symbol cannot be null or empty.");
        if (indicatorList == null || indicatorList.Count == 0)
            throw new ArgumentException("The list of indicators cannot be null or empty.");

    }//end ValidateSymbolAndIndicatorList()


    /// <summary>
    /// Validates the bulk construct.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="RateLimitExceededException"></exception>
    /// <exception cref="Exception"></exception>
    private async Task HandleHttpErrors(HttpResponseMessage response) {

        if (response.StatusCode == HttpStatusCode.Unauthorized) {
            _logger?.LogWarning("Unauthorized. Invalid API key.");
            throw new UnauthorizedAccessException("Unauthorized. Invalid API key.");
        }
        else if (response.StatusCode == HttpStatusCode.TooManyRequests) {
            var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? _retryAfterSeconds;
            _logger?.LogWarning($"Rate limit exceeded. Retry after {retryAfter} seconds.");
            throw new RateLimitExceededException($"Rate limit exceeded. Retry after {retryAfter} seconds.", retryAfter);
        }
        else if (!response.IsSuccessStatusCode) {
            var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _logger?.LogError($"Error fetching indicator: {response.StatusCode}, {errorContent}");
            throw new Exception($"Error fetching indicator: {response.StatusCode}, {errorContent}");
        }

    }//end HandleHttpErrors()

    #endregion


}// class
