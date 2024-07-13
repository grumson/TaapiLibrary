using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response;
public class TaapiBulkDataIndicatorResponse {

    public int? backtrack { get; set; }


    // RSI, SMA, EMA, SuperTrend
    public double? value { get; set; }


    // SuperTrend
    public string? valueAdvice { get; set; }


    // MACD
    public double? valueMACD { get; set; }
    public double? valueMACDSignal { get; set; }
    public double? valueMACDHist { get; set; }


    // Stochastic
    public double? valueK { get; set; }
    public double? valueD { get; set; }


    // BBands
    public double? valueUpperBand { get; set; }
    public double? valueMiddleBand { get; set; }
    public double? valueLowerBand { get; set; }


}// class
