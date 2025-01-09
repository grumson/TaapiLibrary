using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Core.Models;
/// <summary>
/// Represents a request for fetching an indicator from the Taapi.io API.
/// </summary>
public class IndicatorRequest {


    #region *** PROPERTIES ***

    // Mandatory parameters
    public string ApiKey { get; set; } = string.Empty; // The API secret key.
    public string Exchange { get; set; } = string.Empty; // The exchange, e.g., "binance".
    public string Symbol { get; set; } = string.Empty; // The trading pair, e.g., "BTC/USDT".
    public string Interval { get; set; } = string.Empty; // The time frame, e.g., "1h".
    public string Indicator { get; set; } = string.Empty; // The indicator type, e.g. ema, rsi,...

    // Optional parameters
    public int? Backtrack { get; set; } // Number of candles to backtrack.
    public string? Chart { get; set; } // Chart type: "candles" or "heikinashi".
    public string? Type { get; set; } // Asset class: "crypto" or "stocks".
    public int? Results { get; set; } // Number of results to fetch.
    public bool? AddResultTimestamp { get; set; } // Include timestamps in results.
    public bool? Gaps { get; set; } // Ensure no gaps in data.

    // Additional parameters specific to certain indicators
    public Dictionary<string, string> AdditionalParameters { get; set; } = new();

    #endregion



    #region *** PUBLIC METHODS ***

    /// <summary>
    /// Constructs the query string for the API call.
    /// </summary>
    /// <returns>A string representing the query parameters.</returns>
    public string ToQueryString() {

        var queryParameters = new List<string>
        {
                $"secret={ApiKey}",
                $"exchange={Exchange}",
                $"symbol={Symbol}",
                $"interval={Interval}"
            };

        if (Backtrack.HasValue) queryParameters.Add($"backtrack={Backtrack}");
        if (!string.IsNullOrEmpty(Chart)) queryParameters.Add($"chart={Chart}");
        if (!string.IsNullOrEmpty(Type)) queryParameters.Add($"type={Type}");
        if (Results.HasValue) queryParameters.Add($"results={Results}");
        if (AddResultTimestamp.HasValue) queryParameters.Add($"addResultTimestamp={AddResultTimestamp.Value.ToString().ToLower()}");
        if (Gaps.HasValue) queryParameters.Add($"gaps={Gaps.Value.ToString().ToLower()}");

        // Add additional parameters
        if (AdditionalParameters != null && AdditionalParameters.Any()) {
            queryParameters.AddRange(
                AdditionalParameters.Select(kv => $"{kv.Key}={kv.Value}")
            );
        }

        return string.Join("&", queryParameters);
    }//end ToQueryString()

    #endregion


}// class
