using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Core.Defaults;
using TaapiLibrary.Core.Models;
using TaapiLibrary.Exceptions;
using TaapiLibrary.Helpers;

namespace TaapiLibrary.Services;
/// <summary>
/// Client for interacting with the Taapi.io API. Provides methods for fetching indicators,
/// bulk requests, and testing the connection while respecting API rate limits and logging activities.
/// </summary>
public class TaapiClient : ITaapiClient {


    #region *** PROPERTIES ***

    private readonly HttpClient _httpClient;
    private readonly RateLimiter _rateLimiter;
    private readonly ILogger<TaapiClient>? _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly SubscriptionType _subscriptionType;

    #endregion



    #region *** CONSTRUCTOR ***
    /// <summary>
    /// Initializes a new instance of the <see cref="TaapiClient"/> class.
    /// </summary>
    /// <param name="apiKey">The API key for authenticating with the Taapi.io API.</param>
    /// <param name="baseUrl">The base URL of the Taapi.io API.</param>
    /// <param name="subscriptionType">The subscription type (e.g., Free, Basic, Pro, Enterprise).</param>
    /// <param name="logger">The logger instance for logging activities.</param>
    public TaapiClient(ILogger<TaapiClient>? logger, string apiKey, SubscriptionType subscriptionType, string baseUrl = "https://api.taapi.io") {

        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _logger = logger;
        _subscriptionType = subscriptionType;
        _apiKey = apiKey;
        _baseUrl = baseUrl;

        int maxRequestsPerMinute = GetRateLimitForSubscription(subscriptionType);
        _rateLimiter = new RateLimiter(maxRequestsPerMinute, TimeSpan.FromMinutes(1));
    }
    #endregion



    #region *** PUBLIC METHODS ***

    /// <summary>
    /// Fetches data for a specific indicator.
    /// </summary>
    /// <param name="request">The request object containing the indicator parameters.</param>
    /// <returns>An <see cref="IndicatorResponse"/> containing the indicator data.</returns>
    public async Task<IndicatorResponse> GetIndicatorAsync(IndicatorRequest request) {

        _logger?.LogInformation("Fetching indicator {Indicator} data for {Symbol} on {Exchange} with interval {Interval}.", request.Indicator, request.Symbol, request.Exchange, request.Interval);

        if (string.IsNullOrWhiteSpace(request.Indicator)) {
            throw new TaapiException("Indicator type must be specified in the request.");
        }

        try {

            await _rateLimiter.WaitToProceedAsync();

            request.ApiKey = _apiKey;

            // Pravilno sestavljanje URL-ja z indikatorjem
            var url = $"{_baseUrl}/{request.Indicator}?{request.ToQueryString()}";

            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                HandleError(response, responseBody);
            }

            _logger?.LogInformation("Successfully fetched indicator {Indicator} data for {Symbol}.", request.Indicator, request.Symbol);
            return IndicatorResponse.FromJson(responseBody, request.Indicator);
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Error fetching indicator {Indicator} data for {Symbol} on {Exchange}.", request.Indicator, request.Symbol, request.Exchange);
            throw;
        }
        finally {
            _rateLimiter.Release();
        }

    }//end GetIndicatorAsync()


    /// <summary>
    /// Fetches data for multiple indicators in a bulk request.
    /// </summary>
    /// <param name="requestBulk">The bulk request object containing multiple indicator parameters.</param>
    /// <returns>A <see cref="BulkResponse"/> containing the data for all requested indicators.</returns>
    public async Task<BulkResponse> GetBulkAsync(BulkRequest requestBulk) {

        _logger?.LogInformation("Fetching bulk data for {ConstructCount} constructs.", requestBulk.Constructs.Count);

        try {

            await _rateLimiter.WaitToProceedAsync();

            // Set the API key
            requestBulk.ApiKey = _apiKey;

            // Set the maximum Constructs limits for the subscription type
            requestBulk.MaxConstructs = GetConstructLimitForSubscription(_subscriptionType);
            // Validate the request limits
            requestBulk.ValidateLimits();

            // Pripravi telo zahteve
            var requestBody = new {
                secret = _apiKey,
                construct = requestBulk.Constructs.Select(construct => new {
                    exchange = construct.Exchange,
                    symbol = construct.Symbol,
                    interval = construct.Interval,
                    indicators = construct.Indicators.Select(indicator => new {
                        indicator = indicator.Indicator,
                        id = indicator.Id,
                        backtrack = indicator.Backtrack,
                        chart = indicator.Chart,
                        results = indicator.Results,
                        addResultTimestamp = indicator.AddResultTimestamp,
                        gaps = indicator.Gaps,
                        additionalParameters = indicator.AdditionalParameters
                    })
                })
            };

            // Pošlji zahtevo kot JSON
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/bulk", requestBody);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                HandleError(response, responseBody);
            }

            _logger?.LogInformation("Successfully fetched bulk data.");
            return BulkResponse.FromJson(responseBody);
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Error fetching bulk data.");
            throw;
        }
        finally {
            _rateLimiter.Release();
        }

    }//end GetBulkAsync()


    /// <summary>
    /// Tests the connection to the Taapi.io API.
    /// </summary>
    /// <returns>True if the connection is successful; otherwise, false.</returns>
    public async Task<bool> TestConnectionAsync() {

        _logger?.LogInformation("Testing connection to Taapi.io API.");

        try {

            // Wait for the rate limiter to allow the request
            await _rateLimiter.WaitToProceedAsync();

            // Send a test request to the API
            var response = await _httpClient.GetAsync($"{_baseUrl}/rsi?secret={_apiKey}&exchange=binance&symbol=BTC/USDT&interval=1h");
            bool isSuccess = response.IsSuccessStatusCode;

            if (isSuccess) {
                _logger?.LogInformation("Successfully connected to Taapi.io API.");
            }
            else {
                _logger?.LogWarning("Failed to connect to Taapi.io API.");
            }

            return isSuccess;
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Error testing connection to Taapi.io API.");
            throw;
        }
        finally {
            _rateLimiter.Release();
        }

    }//end TestConnectionAsync()

    #endregion



    #region *** PRIVATE METHODS ***

    /// <summary>
    /// Handles API errors by parsing the response and throwing a <see cref="TaapiException"/>.
    /// </summary>
    /// <param name="responseBody">The raw response body from the API.</param>
    private void HandleError(HttpResponseMessage response, string responseBody) {

        _logger?.LogError("HTTP {StatusCode}: {ReasonPhrase}. Response: {ResponseBody}",
       response.StatusCode,
       response.ReasonPhrase,
       responseBody);

        ErrorHandler.HandleApiError(responseBody, _logger);

    }//end HandleError()


    /// <summary>
    /// Retrieves the API rate limit for the specified subscription type.
    /// </summary>
    /// <param name="subscriptionType">The subscription type.</param>
    /// <returns>The rate limit in requests per minute.</returns>
    private int GetRateLimitForSubscription(SubscriptionType subscriptionType) {

        return subscriptionType switch {
            SubscriptionType.Free => ApiRateLimits.RateLimitsFree,
            SubscriptionType.Basic => ApiRateLimits.RateLimitsBasic,
            SubscriptionType.Pro => ApiRateLimits.RateLimitsPro,
            SubscriptionType.Enterprise => ApiRateLimits.RateLimitsExpert,
            _ => throw new ArgumentOutOfRangeException(nameof(subscriptionType), "Invalid subscription type")
        };
    }//end GetRateLimitForSubscription()


    /// <summary>
    /// Retrieves the maximum construct limit for the specified subscription type.
    /// </summary>
    /// <param name="subscriptionType">The subscription type.</param>
    /// <returns>The maximum number of constructs allowed.</returns>
    private int GetConstructLimitForSubscription(SubscriptionType subscriptionType) {

        return subscriptionType switch {
            SubscriptionType.Free => ApiRateLimits.ConstructLimitsFree,
            SubscriptionType.Basic => ApiRateLimits.ConstructLimitsBasic,
            SubscriptionType.Pro => ApiRateLimits.ConstructLimitsPro,
            SubscriptionType.Enterprise => ApiRateLimits.ConstructLimitsExpert,
            _ => throw new ArgumentOutOfRangeException(nameof(subscriptionType), "Invalid subscription type")
        };
    }//end GetConstructLimitForSubscription()

    #endregion


}// class
