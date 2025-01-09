using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Core.Enums;
public enum ExchangeType {


    [JsonProperty("binance")]
    [Description("binance")]
    Binance,

    [JsonProperty("binancefutures")]
    [Description("binancefutures")]
    BinanceFutures,

    [JsonProperty("coinbase")]
    [Description("coinbase")]
    Coinbase,

    [JsonProperty("kraken")]
    [Description("kraken")]
    Kraken,

    [JsonProperty("bitstamp")]
    [Description("bitstamp")]
    Bitstamp,

    [JsonProperty("whitebit")]
    [Description("whitebit")]
    WhiteBIT,

    [JsonProperty("bybit")]
    [Description("bybit")]
    ByBit,

    [JsonProperty("gateio")]
    [Description("gateio")]
    GateIo,

    [JsonProperty("binanceus")]
    [Description("binanceus")]
    BinanceUs,


}// enum
