using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Contracts.Response.Bulk;
public class TaapiIndicatorValuesResponse
{

    public int? backtrack { get; set; }


    // RSI, SMA, EMA, SuperTrend, ATR, MA
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


    // StochRsi
    public double? valueFastK { get; set; }
    public double? valueFastD { get; set; }


    // DMI
    public double? Adx { get; set; }
    public double? Pdi { get; set; }
    public double? Mdi { get; set; }


    // CANDLE
    public double? volume { get; set; }
    public string? timestampHuman { get; set; }
    public int? timestamp { get; set; }
    public double? open { get; set; }
    public double? close { get; set; }
    public double? high { get; set; }
    public double? low { get; set; }


}// class
