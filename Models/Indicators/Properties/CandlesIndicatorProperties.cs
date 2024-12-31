using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
using TaapiLibrary.Enums;

namespace TaapiLibrary.Models.Indicators.Properties;
public class CandlesIndicatorProperties : ICandlesIndicatorProperties {


    #region *** PROPERTIES ***

    public string Id { get; set; } = string.Empty;
    public TaapiIndicatorType Indicator { get; private set; } = TaapiIndicatorType.Candles;
    public TaapiChart Chart { get; set; }
    public int? Backtrack { get; set; }
    public bool? ChartGaps { get; set; }
    public bool? AddResultTimestamp { get; set; }
    public string? FromTimestamp { get; set; }
    public string? ToTimestamp { get; set; }
    public string? Results { get; set; }

    public int? Period { get; set; } // Number of candles you want to return. Maximum 100.

    public const int DefaultPeriod = 14;

    #endregion


}// class
