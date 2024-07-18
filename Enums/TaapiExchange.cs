using Newtonsoft.Json;
using System.ComponentModel;


namespace TaapiLibrary.Enums;
public enum TaapiExchange {

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
