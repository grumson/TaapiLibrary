using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Requests;
using TaapiLibrary.Contracts.Requests.Interfaces;
using TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
using TaapiLibrary.Contracts.Response.Bulk;
using TaapiLibrary.Enums;
using TaapiLibrary.Exceptions;

namespace TaapiLibrary;
public class TaapiClient {


    #region *** PROPERTIES ***

    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly string _baseUrl = "https://api.taapi.io";
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

    // Get Indicator directly
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


    // Post Bulk Indicators
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


    // Create a Bulk Request
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


    // Create a Bulk Construct
    public TaapiBulkConstruct CreateBulkConstruct(TaapiExchange exchange, string symbol, TaapiCandlesInterval candlesInterval, List<ITaapiIndicatorRequest> indicatorList) {


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

    // Map ITaapiIndicatorRequest to TaapiIndicatorPropertiesRequest
    private TaapiIndicatorPropertiesRequest MapIndicatorRequest(ITaapiIndicatorRequest indicatorRequest) {

        TaapiIndicatorPropertiesRequest taapiIndicatorPropertiesRequest = null!;
            
        // RSI
        if (indicatorRequest is IRsiIndicatorResponse rsiIndicatorResponse) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                Period = rsiIndicatorResponse.Period,
            };
        }
        // MACD
        else if (indicatorRequest is IMacdIndicatorResponse macdIndicatorResponse) {

            taapiIndicatorPropertiesRequest = new TaapiIndicatorPropertiesRequest(indicatorRequest.Indicator, indicatorRequest.Chart) {
                OptInFastPeriod = macdIndicatorResponse.OptInFastPeriod,
                OptInSlowPeriod = macdIndicatorResponse.OptInSlowPeriod,
                OptInSignalPeriod = macdIndicatorResponse.OptInSignalPeriod,
            };
        }
        else {

            throw new NotImplementedException("The indicator is not implemented.");
        }


        return taapiIndicatorPropertiesRequest;
    }//end MapIndicatorRequest()

    #endregion


}// class
