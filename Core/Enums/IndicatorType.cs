﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Core.Enums;
public enum IndicatorType {

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

    [JsonProperty("atr")]
    [Description("atr")]
    Atr,

    [JsonProperty("stochrsi")]
    [Description("stochrsi")]
    StochRsi,

    [JsonProperty("ma")]
    [Description("ma")]
    Ma,

    [JsonProperty("dmi")]
    [Description("dmi")]
    Dmi,

    [JsonProperty("candle")]
    [Description("candle")]
    Candle,

    [JsonProperty("candles")]
    [Description("candles")]
    Candles,

    [JsonProperty("fibonacciretracement")]
    [Description("fibonacciretracement")]
    FibonacciRetracement,

    [JsonProperty("stddev")]
    [Description("stddev")]
    StandardDeviation,

    [JsonProperty("adx")]
    [Description("adx")]
    AverageDirectionalMovement,

    [JsonProperty("wad")]
    [Description("wad")]
    WilliamsAccumulationDistribution,

    [JsonProperty("ichimoku")]
    [Description("ichimoku")]
    IchimokuCloud,

    [JsonProperty("cci")]
    [Description("cci")]
    CommodityChannelIndex,

    [JsonProperty("obv")]
    [Description("obv")]
    OnBalanceVolume,

    [JsonProperty("psar")]
    [Description("psar")]
    ParabolicSar,

    [JsonProperty("willr")]
    [Description("willr")]
    WilliamsR,

    [JsonProperty("mfi")]
    [Description("mfi")]
    MoneyFlowIndex,

    [JsonProperty("ppo")]
    [Description("ppo")]
    PercentagePriceOscillator,

    [JsonProperty("cmf")]
    [Description("cmf")]
    ChaikinMoneyFlow,

    [JsonProperty("volume")]
    [Description("volume")]
    Volume,

    [JsonProperty("vwap")]
    [Description("vwap")]
    VolumeWeightedAveragePrice,

    [JsonProperty("aroon")]
    [Description("aroon")]
    Aroon,

    [JsonProperty("adxr")]
    [Description("adxr")]
    AverageDirectionalMovementIndexRating,

    [JsonProperty("aroonosc")]
    [Description("aroonosc")]
    AroonOscillator,

    [JsonProperty("bop")]
    [Description("bop")]
    BalanceOfPower,

    [JsonProperty("cmo")]
    [Description("cmo")]
    ChandeMomentumOscillator,

}// enum
