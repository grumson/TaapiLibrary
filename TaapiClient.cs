using Newtonsoft.Json;
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

    #endregion



    #region *** CONSTRUCTORS ***
    public TaapiClient(string baseUrl = "https://api.taapi.io", int retryAfterSeconds = 60) {

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

        // check if the symbol is null or empty
        if (string.IsNullOrEmpty(symbol)) {
            throw new ArgumentException("The symbol cannot be null or empty.");
        }

        // check if the apiKey is null or empty
        if (string.IsNullOrEmpty(apiKey)) {
            throw new ArgumentException("The API key cannot be null or empty.");
        }

        // check if the TaapiIndicatorPropertiesRequest is null or empty
        if (directParametersRequest == null) {
            throw new ArgumentException("The TaapiIndicatorPropertiesRequest cannot be null or empty.");
        }

        // Set the Mandatory Parameters
        var parametersMandatory = $"exchange={exchange.GetDescription()}&symbol={symbol}&interval={candlesInterval.GetDescription()}";

        // Set the Optional Parameters
        var parametersOptional = $"&backtrack={directParametersRequest.Backtrack}&gaps={directParametersRequest.Gaps}&period={directParametersRequest.Period}&stddev={directParametersRequest.StdDev}&multiplier={directParametersRequest.Multiplier}&optInFastPeriod={directParametersRequest.OptInFastPeriod}&optInSlowPeriod={directParametersRequest.OptInSlowPeriod}&optInSignalPeriod={directParametersRequest.OptInSignalPeriod}&kPeriod={directParametersRequest.KPeriod}&dPeriod={directParametersRequest.DPeriod}";

        // create the URL
        var url = $"{_baseUrl}/{directParametersRequest.Indicator}?secret={apiKey}&{parametersMandatory}&{parametersOptional}";

        try {

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

            // Get the indicators results from the response
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


    }//end GetBulkIndicatorsResults()


    /// <summary>
    /// Creates a bulk request for multiple indicators.
    /// </summary>
    /// <param name="apiKey">API key for authentication.</param>
    /// <param name="bulkConstructList">List of bulk constructs.</param>
    /// <returns>Bulk request object.</returns>
    public TaapiBulkRequest CreateBulkRequest(string apiKey, List<TaapiBulkConstruct> bulkConstructList) {


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
    }//end CreateBulkConstruct()

    #endregion



    #region *** PRIVATE METHODS ***

    /// <summary>
    /// Maps an indicator request to a TaapiIndicatorPropertiesRequest object.
    /// </summary>
    /// <param name="indicatorRequest">Indicator request to map.</param>
    /// <returns>Mapped TaapiIndicatorPropertiesRequest object.</returns>
    private TaapiIndicatorPropertiesRequest MapIndicatorRequest(ITaapiIndicatorProperties indicatorRequest) {

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
        // Not implemented
        else {

            throw new NotImplementedException("The indicator is not implemented.");
        }

        taapiIndicatorPropertiesRequest.Id = indicatorRequest.Id;
        taapiIndicatorPropertiesRequest.Backtrack = indicatorRequest.Backtrack;
        taapiIndicatorPropertiesRequest.Results = indicatorRequest.Results;
        taapiIndicatorPropertiesRequest.Gaps = indicatorRequest.ChartGaps;

        return taapiIndicatorPropertiesRequest;
    }//end MapIndicatorRequest()


    /// <summary>
    /// Maps a bulk data response to an indicator results object.
    /// </summary>
    /// <param name="taapiBulkDataResponse">Bulk data response to map.</param>
    /// <returns>Mapped indicator results object.</returns>
    private ITaapiIndicatorResults MapIndicatorResults(TaapiBulkDataResponse taapiBulkDataResponse) {

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
        // Not implemented
        else {

            throw new NotImplementedException("The indicator is not implemented.");
        }

        return taapiIndicatorResults;
    }//end MapIndicatorResults()


    /// <summary>
    /// Extracts indicator results from a bulk response.
    /// </summary>
    /// <param name="taapiBulkResponse">Bulk response containing indicator data.</param>
    /// <returns>List of indicator results.</returns>
    private List<ITaapiIndicatorResults> GetIndicatorResults(TaapiBulkResponse taapiBulkResponse) {

        List<ITaapiIndicatorResults> taapiIndicatorResultsList = new List<ITaapiIndicatorResults>();

        if (taapiBulkResponse?.data?.Count > 0) {
            foreach (var taapiBulkDataResponse in taapiBulkResponse.data) {

                ITaapiIndicatorResults taapiIndicatorResults = MapIndicatorResults(taapiBulkDataResponse);

                taapiIndicatorResultsList.Add(taapiIndicatorResults);
            }
        }

        return taapiIndicatorResultsList;
    }//end GetIndicatorResults()

    #endregion


}// class
