using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
using TaapiLibrary.Enums;

namespace TaapiLibrary.Models.Indicators.Properties;
public class StochRsiIndicatorProperties : IStochRsiIndicatorProperties {


    #region *** PROPERTIES ***

    public string Id { get; set; } = string.Empty;
    public TaapiIndicatorType Indicator { get; private set; } = TaapiIndicatorType.StochRsi;
    public TaapiChart Chart { get; set; }
    public int? Backtrack { get; set; }
    public bool? ChartGaps { get; set; }
    public string? Results { get; set; }

    public int? KPeriod { get; set; }
    public int? DPeriod { get; set; }
    public int? RsiPeriod { get; set; }
    public int? StochasticPeriod { get; set; }

    public const int DefaultKPeriod = 5;
    public const int DefaultDPeriod = 3;
    public const int DefaultRsiPeriod = 14;
    public const int DefaultStochasticPeriod = 14;

    #endregion


}// class
