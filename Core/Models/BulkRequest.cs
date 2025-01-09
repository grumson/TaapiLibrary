using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Core.Defaults;
using TaapiLibrary.Helpers;

namespace TaapiLibrary.Core.Models;


/// <summary>
/// Represents a bulk request for fetching multiple indicators from the Taapi.io API.
/// </summary>
public class BulkRequest {

    /// <summary>
    /// The API secret key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// A list of constructs for the request.
    /// </summary>
    public List<Construct> Constructs { get; set; } = new();

    /// <summary>
    /// The maximum allowed number of constructs based on the subscription plan.
    /// </summary>
    public int MaxConstructs { get; set; } = ApiRateLimits.ConstructLimitsBasic; // Default to basic plan

    /// <summary>
    /// The maximum allowed number of calculations per construct.
    /// </summary>
    public int MaxCalculations { get; set; } = ApiRateLimits.MaxCalculations;


    /// <summary>
    /// Validates that the request does not exceed allowed limits.
    /// </summary>
    public void ValidateLimits(ILogger? logger = null) {
        // Prepare the list of calculations per construct
        var calculationsPerConstruct = Constructs.Select(c => c.Indicators.Count).ToList();

        // Delegate validation to ErrorHandler
        ErrorHandler.ValidateLimits(
            Constructs.Count,
            MaxConstructs,
            calculationsPerConstruct,
            MaxCalculations,
            logger
        );

    }//end ValidateLimits()

}// class BulkRequest


/// <summary>
/// Represents the construct part of the bulk request.
/// </summary>
public class Construct {

    /// <summary>
    /// The exchange to fetch data from (e.g., "binance"). Mandatory for crypto.
    /// </summary>
    public string Exchange { get; set; } = string.Empty;

    /// <summary>
    /// The trading pair (e.g., "BTC/USDT").
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// The time frame (e.g., "1h").
    /// </summary>
    public string Interval { get; set; } = string.Empty;

    /// <summary>
    /// The type of asset (e.g., "crypto", "stocks"). Defaults to "crypto".
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// The list of indicators to fetch.
    /// </summary>
    public List<IndicatorDetail> Indicators { get; set; } = new();

}// class Construct


/// <summary>
/// Represents the details of a specific indicator in the bulk request.
/// </summary>
public class IndicatorDetail {

    /// <summary>
    /// The name of the indicator (e.g., "rsi").
    /// </summary>
    public string Indicator { get; set; } = string.Empty;

    /// <summary>
    /// A custom ID to track the result of this indicator call.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The number of candles to backtrack.
    /// </summary>
    public int? Backtrack { get; set; }

    /// <summary>
    /// The chart type (e.g., "candles", "heikinashi"). Defaults to "candles".
    /// </summary>
    public string? Chart { get; set; }

    /// <summary>
    /// The number of results to fetch (max 20).
    /// </summary>
    public int? Results { get; set; }

    /// <summary>
    /// Whether to include a timestamp for each result.
    /// </summary>
    public bool? AddResultTimestamp { get; set; }

    /// <summary>
    /// Whether to ensure no gaps in data.
    /// </summary>
    public bool? Gaps { get; set; }

    /// <summary>
    /// Additional parameters specific to the indicator.
    /// </summary>
    public Dictionary<string, string> AdditionalParameters { get; set; } = new();

}// class IndicatorDetail

