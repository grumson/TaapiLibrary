﻿using Newtonsoft.Json;
using TaapiLibrary.Enums;


namespace TaapiLibrary.Contracts.Requests;
public class TaapiIndicatorPropertiesRequest {


    #region *** PROPERTIES ***

    [JsonProperty("id")]
    public string? Id { get; set; } = string.Empty;

    [JsonProperty("indicator")]
    public string Indicator { get; set; }

    [JsonProperty("backtrack")]
    public int? Backtrack { get; set; }

    [JsonProperty("chart")]
    public string? Chart { get; set; }

    [JsonProperty("gaps")]
    public bool? Gaps { get; set; }

    [JsonProperty("results")]
    public string? Results { get; set; }

    [JsonProperty("addResultTimestamp")]
    public bool? AddResultTimestamp { get; set; }


    // RSI, SMA, EMA, BBands, SuperTrend, ATR, MA, DMI
    [JsonProperty("period")]
    public int? Period { get; set; }


    // BBands
    [JsonProperty("stddev")]
    public int? StdDev { get; set; }


    // SuperTrend
    [JsonProperty("multiplier")]
    public int? Multiplier { get; set; }


    // MACD
    [JsonProperty("optInFastPeriod")]
    public int? OptInFastPeriod { get; set; }

    [JsonProperty("optInSlowPeriod")]
    public int? OptInSlowPeriod { get; set; }

    [JsonProperty("optInSignalPeriod")]
    public int? OptInSignalPeriod { get; set; }


    // Stochastic, StochRsi
    [JsonProperty("kPeriod")]
    public int? KPeriod { get; set; }

    [JsonProperty("dPeriod")]
    public int? DPeriod { get; set; }


    // Stochastic
    [JsonProperty("kSmooth")]
    public int? KSmooth { get; set; }


    // StochRsi
    [JsonProperty("RsiPeriod")]
    public int? RsiPeriod { get; set; }

    [JsonProperty("StochasticPeriod")]
    public int? StochasticPeriod { get; set; }


    // Candles
    [JsonProperty("fromTimestamp")]
    public string? FromTimestamp { get; set; }

    [JsonProperty("toTimestamp")]
    public string? ToTimestamp { get; set; }


    // Fibonacci Retracement
    [JsonProperty("retracement")]
    public float? Retracement { get; set; }

    [JsonProperty("trend")]
    public string? Trend { get; set; }


    // Stddev
    [JsonProperty("optInNbDev")]
    public float? OptInNbDev { get; set; }

    #endregion


    #region *** CONSTRUCTORS ***

    public TaapiIndicatorPropertiesRequest(TaapiIndicatorType indicator, TaapiChart chart) {
        Indicator = indicator.GetDescription();
        Chart = chart.GetDescription();
    }

    #endregion


}// class
