﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
using TaapiLibrary.Enums;

namespace TaapiLibrary.Models.Indicators.Properties;
public class StandardDeviationIndicatorProperties : IStandardDeviationIndicatorProperties {


    #region *** PROPERTIES ***

    public string Id { get; set; } = string.Empty;
    public TaapiIndicatorType Indicator { get; private set; } = TaapiIndicatorType.StandardDeviation;
    public TaapiChart Chart { get; set; }
    public int? Backtrack { get; set; }
    public bool? ChartGaps { get; set; }
    public bool? AddResultTimestamp { get; set; }
    public string? FromTimestamp { get; set; }
    public string? ToTimestamp { get; set; }
    public string? Results { get; set; }

    public int? Period { get; set; }
    public float? OptInNbDev { get; set; }

    public const int DefaultPeriod = 5;
    public const float DefaultOptInNbDev = 1.0f;

    #endregion


}// class
