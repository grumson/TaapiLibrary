using Newtonsoft.Json;
using TaapiLibrary.Enums;


namespace TaapiLibrary.Contracts.Requests;
public class TaapiIndicatorPropertiesRequest
{


    #region *** PROPERTIES ***

    [JsonProperty("id")]
    public string? Id { get; private set; } = string.Empty;

    [JsonProperty("indicator")]
    public string Indicator { get; private set; }

    [JsonProperty("backtrack")]
    public int? Backtrack { get; private set; }

    [JsonProperty("chart")]
    public string? Chart { get; private set; }

    [JsonProperty("gaps")]
    public bool? Gaps { get; private set; }


    // RSI, SMA, EMA, BBands, SuperTrend, ATR, MA, DMI
    [JsonProperty("period")]
    public int? Period { get; private set; }


    // BBands
    [JsonProperty("stddev")]
    public int? StdDev { get; private set; }


    // SuperTrend
    [JsonProperty("multiplier")]
    public int? Multiplier { get; private set; }


    // MACD
    [JsonProperty("optInFastPeriod")]
    public int? OptInFastPeriod { get; private set; }

    [JsonProperty("optInSlowPeriod")]
    public int? OptInSlowPeriod { get; private set; }

    [JsonProperty("optInSignalPeriod")]
    public int? OptInSignalPeriod { get; private set; }


    // Stochastic, StochRsi
    [JsonProperty("kPeriod")]
    public int? KPeriod { get; private set; }

    [JsonProperty("dPeriod")]
    public int? DPeriod { get; private set; }


    // Stochastic
    [JsonProperty("kSmooth")]
    public int? KSmooth { get; private set; }


    // StochRsi
    [JsonProperty("rsiPeriod")]
    public int? rsiPeriod { get; set; }

    [JsonProperty("stochasticPeriod")]
    public int? stochasticPeriod { get; set; }

    #endregion


    #region *** CONSTRUCTORS ***
    public TaapiIndicatorPropertiesRequest(TaapiIndicatorType indicator, TaapiChart chart, string? id = null, int? backtrack = null, bool? gaps = null, int? period = null, int? stddev = null, int? multiplier = null, int? optInFastPeriod = null, int? optInSlowPeriod = null, int? optInSignalPeriod = null, int? kPeriod = null, int? dPeriod = null, int? kSmooth = null)
    {

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

    }
    #endregion


}// class
