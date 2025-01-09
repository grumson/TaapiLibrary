using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Core.Enums;
public enum ChartType {


    [JsonProperty("candles")]
    [Description("candles")]
    Candles,

    [JsonProperty("heikinashi")]
    [Description("heikinashi")]
    Heikinashi,


}// enum
