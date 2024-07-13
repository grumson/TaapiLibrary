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

    [JsonProperty("bitfinex")]
    [Description("bitfinex")]
    Bitfinex,

}// enum
