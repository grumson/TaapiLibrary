using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Requests;
using TaapiLibrary.Contracts.Response.Bulk;
using TaapiLibrary.Enums;
using TaapiLibrary.Exceptions;

namespace TaapiLibrary;
public class TaapiClient {


    #region *** PROPERTIES ***

    private readonly HttpClient _httpClient = null!;
    private readonly string _baseUrl = "https://api.taapi.io";
    private readonly int _retryAfterSeconds;

    #endregion



    #region *** CONSTRUCTORS ***
    public TaapiClient(int _retryAfterSeconds = 60) {

        _httpClient = new HttpClient();
    }
    #endregion



    #region *** PUBLIC METHODS ***

    // Get Indicator directly
    public async Task<T> GetIndicator<T>(string apiKey, string symbol, TaapiExchange exchange, TaapiCandlesInterval candlesInterval, TaapiIndicatorPropertiesRequest directParametersRequest ) {

        // Set the Mandatory Parameters
        var parametersMandatory = $"exchange={exchange.GetDescription()}&symbol={symbol}&interval={candlesInterval.GetDescription()}";

        // Set the Optional Parameters
        var parametersOptional = $"&backtrack={directParametersRequest.Backtrack}&gaps={directParametersRequest.Gaps}&period={directParametersRequest.Period}&stddev={directParametersRequest.StdDev}&multiplier={directParametersRequest.Multiplier}&optInFastPeriod={directParametersRequest.OptInFastPeriod}&optInSlowPeriod={directParametersRequest.OptInSlowPeriod}&optInSignalPeriod={directParametersRequest.OptInSignalPeriod}&kPeriod={directParametersRequest.KPeriod}&dPeriod={directParametersRequest.DPeriod}";

        // create the URL
        var url = $"{_baseUrl}/{directParametersRequest.Indicator}?secret={apiKey}&{parametersMandatory}&{parametersOptional}";

        // Send the request
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(jsonString);

    }//end GetIndicator()


    // Post Bulk Indicators
    public async Task<List<TaapiBulkResponse>> PostBulkIndicatorsAsync(TaapiBulkRequest requests) {

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
                var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? 60; // Predpostavimo 60 sekund, če Retry-After glava ni podana
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

    #endregion



    #region *** PRIVATE METHODS ***

    #endregion


}// class
