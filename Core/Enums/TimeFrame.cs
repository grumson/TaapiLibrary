using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Core.Enums;
public enum TimeFrame {


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
