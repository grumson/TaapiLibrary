﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Requests.Interfaces.Indicators;
using TaapiLibrary.Enums;

namespace TaapiLibrary.Models.IndicatorProperties;
public class RsiIndicatorPropertie : IRsiIndicatorResponse {


    #region *** PROPERTIES ***

    public string Id { get; set; } = string.Empty;
    public TaapiIndicatorType Indicator { get; set; }
    public TaapiChart Chart { get; set; }
    public string? Backtrack { get; set; }
    public bool? ChartGaps { get; set; }
    public string? Results { get; set; }
    public int? Period { get; set; }

    #endregion

}// class