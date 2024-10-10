using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaapiLibrary.Contracts.Response.Bulk.Interfaces.Indicators;

namespace TaapiLibrary.Models.Indicators.Results;
public class CandleIndicatorResults : ICandleIndicatorResults {


    #region *** PROPERTIES ***

    public string Id { get; set; } = string.Empty;
    public string Indicator { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new List<string>();

    public double? Volume { get; set; }
    public string? TimestampHuman { get; set; }
    public int? Timestamp { get; set; }
    public double? Open { get; set; }
    public double? Close { get; set; }
    public double? High { get; set; }
    public double? Low { get; set; }

    #endregion


}// class
