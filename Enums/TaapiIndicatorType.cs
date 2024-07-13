﻿using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace TaapiLibrary.Enums;
public enum TaapiIndicatorType {

    [JsonProperty("rsi")]
    [Description("rsi")]
    RSI,

    [JsonProperty("macd")]
    [Description("macd")]
    MACD,

    [JsonProperty("sma")]
    [Description("sma")]
    SMA,

    [JsonProperty("ema")]
    [Description("ema")]
    EMA,

    [JsonProperty("stoch")]
    [Description("stoch")]
    Stochastic,

    [JsonProperty("bbands")]
    [Description("bbands")]
    BBands,

    [JsonProperty("supertrend")]
    [Description("supertrend")]
    SuperTrend,

}// enum


