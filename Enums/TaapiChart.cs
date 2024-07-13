using Newtonsoft.Json;
using System.ComponentModel;

namespace TaapiLibrary.Enums;
public enum TaapiChart {

    [JsonProperty("candles")]
    [Description("candles")]
    Candles,

    [JsonProperty("heikinashi")]
    [Description("heikinashi")]
    Heikinashi,

}// enum
