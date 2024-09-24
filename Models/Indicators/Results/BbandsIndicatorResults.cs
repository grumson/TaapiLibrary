﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;

namespace TaapiLibrary.Models.Indicators.Results;
public class BbandsIndicatorResults : IBbandsIndicatorResults {


    #region *** PROPERTIES ***

    public string Id { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new List<string>();

    public double? ValueUpperBand { get; set; }
    public double? ValueMiddleBand { get; set; }
    public double? ValueLowerBand { get; set; }

    #endregion


}// class
