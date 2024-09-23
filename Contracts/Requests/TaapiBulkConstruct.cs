using Newtonsoft.Json;
using TaapiLibrary.Enums;

namespace TaapiLibrary.Contracts.Requests;
public class TaapiBulkConstruct {


    #region *** PROPERTIES ***

    // Construct data for multiple indicators request

    [JsonProperty("exchange")]
    public string Exchange { get; private set; }

    [JsonProperty("symbol")]
    public string Symbol { get; private set; }

    [JsonProperty("interval")]
    public string Interval { get; private set; }

    [JsonProperty("type")]
    public string Type { get; private set; } = "crypto";


    // List of indicators to be requested for this construct
    [JsonProperty("indicators")]
    public List<TaapiIndicatorPropertiesRequest> Indicators { get; private set; }

    #endregion



    #region *** CONSTRUCTORS ***
    public TaapiBulkConstruct(TaapiExchange exchange, string symbol, TaapiCandlesInterval interval, List<TaapiIndicatorPropertiesRequest> indicators) {

        // check if the symbol is null or empty
        if (string.IsNullOrEmpty(symbol)) {
            throw new ArgumentException("The symbol cannot be null or empty.");
        }
        // check if the list is null or empty
        if (indicators == null || indicators.Count == 0) {
            throw new ArgumentException("The list of indicators cannot be null or empty.");
        }


        // Set the Construct Parameters
        Exchange = exchange.GetDescription();
        Symbol = symbol;
        Interval = interval.GetDescription();
        Indicators = indicators;

    }
    #endregion


}// class
