using Newtonsoft.Json;
using TaapiLibrary.Enums;


namespace TaapiLibrary.Contracts.Requests;
public class TaapiIndicatorPropertiesRequest {


    #region *** PROPERTIES ***

    [JsonProperty("id")]
    public string? Id { get; set; } = string.Empty;

    [JsonProperty("indicator")]
    public string Indicator { get; set; }

    [JsonProperty("backtrack")]
    public int? Backtrack { get; set; }

    [JsonProperty("chart")]
    public string? Chart { get; set; }

    [JsonProperty("gaps")]
    public bool? Gaps { get; set; }


    // RSI, SMA, EMA, BBands, SuperTrend, ATR, MA, DMI
    [JsonProperty("period")]
    public int? Period { get; set; }


    // BBands
    [JsonProperty("stddev")]
    public int? StdDev { get; set; }


    // SuperTrend
    [JsonProperty("multiplier")]
    public int? Multiplier { get; set; }


    // MACD
    [JsonProperty("optInFastPeriod")]
    public int? OptInFastPeriod { get; set; }

    [JsonProperty("optInSlowPeriod")]
    public int? OptInSlowPeriod { get; set; }

    [JsonProperty("optInSignalPeriod")]
    public int? OptInSignalPeriod { get; set; }


    // Stochastic, StochRsi
    [JsonProperty("kPeriod")]
    public int? KPeriod { get; set; }

    [JsonProperty("dPeriod")]
    public int? DPeriod { get; set; }


    // Stochastic
    [JsonProperty("kSmooth")]
    public int? KSmooth { get; set; }


    // StochRsi
    [JsonProperty("RsiPeriod")]
    public int? RsiPeriod { get; set; }

    [JsonProperty("StochasticPeriod")]
    public int? StochasticPeriod { get; set; }

    #endregion


    #region *** CONSTRUCTORS ***
    public TaapiIndicatorPropertiesRequest(TaapiIndicatorType indicator, TaapiChart chart, string? id = null, int? backtrack = null,
        bool? gaps = null, int? period = null, int? stddev = null, int? multiplier = null, int? optInFastPeriod = null, int? optInSlowPeriod = null,
        int? optInSignalPeriod = null, int? kPeriod = null, int? dPeriod = null, int? kSmooth = null, int? rsiPeriod = null, int? stochasticPeriod = null) {

        Id = id;
        Indicator = indicator.GetDescription();
        Chart = chart.GetDescription();
        Backtrack = backtrack;
        Gaps = gaps;
        Period = period;
        StdDev = stddev;
        Multiplier = multiplier;
        OptInFastPeriod = optInFastPeriod;
        OptInSlowPeriod = optInSlowPeriod;
        OptInSignalPeriod = optInSignalPeriod;
        KPeriod = kPeriod;
        DPeriod = dPeriod;
        KSmooth = kSmooth;
        RsiPeriod = rsiPeriod;
        StochasticPeriod = stochasticPeriod;

    }
    #endregion


}// class
