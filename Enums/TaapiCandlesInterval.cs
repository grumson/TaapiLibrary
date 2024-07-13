using Newtonsoft.Json;
using System.ComponentModel;

namespace TaapiLibrary.Enums;
public enum TaapiCandlesInterval {

    // 1m, 5m, 15m, 30m, 1h, 2h, 4h, 12h, 1d, 1w

    [JsonProperty("1m")]
    [Description("1m")]
    OneMinute,

    [JsonProperty("5m")]
    [Description("5m")]
    FiveMinutes,

    [JsonProperty("15m")]
    [Description("15m")]
    FifteenMinutes,

    [JsonProperty("30m")]
    [Description("30m")]
    ThirtyMinutes,

    [JsonProperty("1h")]
    [Description("1h")]
    OneHour,

    [JsonProperty("2h")]
    [Description("2h")]
    TwoHours,

    [JsonProperty("4h")]
    [Description("4h")]
    FourHours,

    [JsonProperty("12h")]
    [Description("12h")]
    TwelveHours,

    [JsonProperty("1d")]
    [Description("1d")]
    OneDay,

    [JsonProperty("1w")]
    [Description("1w")]
    OneWeek

}// enum
