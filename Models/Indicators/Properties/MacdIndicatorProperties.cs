﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
using TaapiLibrary.Enums;

namespace TaapiLibrary.Models.Indicators.Properties;
public class MacdIndicatorProperties : IMacdIndicatorProperties {


    #region *** PROPERTIES ***

    public string Id { get; set; } = string.Empty;
    public TaapiIndicatorType Indicator { get; private set; } = TaapiIndicatorType.MACD;
    public TaapiChart Chart { get; set; }
    public int? Backtrack { get; set; }
    public bool? ChartGaps { get; set; }
    public string? Results { get; set; }

    public int? OptInFastPeriod { get; set; }
    public int? OptInSlowPeriod { get; set; }
    public int? OptInSignalPeriod { get; set; }

    public const int DefaultOptInFastPeriod = 12;
    public const int DefaultOptInSlowPeriod = 26;
    public const int DefaultOptInSignalPeriod = 9;

    #endregion


}// class
