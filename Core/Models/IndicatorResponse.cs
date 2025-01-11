using System.Text.Json;

namespace TaapiLibrary.Core.Models;
/// <summary>
/// Represents the response from the Taapi.io API for an indicator.
/// </summary>
public class IndicatorResponse {

    public Dictionary<string, object> Data { get; private set; } = new();

    public List<CandleData>? Candles { get; private set; }

    /// <summary>
    /// Deserializes the JSON response from the Taapi.io API into an <see cref="IndicatorResponse"/>.
    /// </summary>
    /// <param name="jsonResponse"></param>
    /// <param name="indicator"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static IndicatorResponse FromJson(string jsonResponse, string indicator = "") {

        if (string.IsNullOrWhiteSpace(jsonResponse)) {
            throw new ArgumentException("Response cannot be null or empty", nameof(jsonResponse));
        }

        try {

            // Check if the indicator is for candles
            if (indicator.Equals("candles", StringComparison.OrdinalIgnoreCase)) {

                // Deserialize the JSON array into a list of CandleData
                var candles = JsonSerializer.Deserialize<List<CandleData>>(jsonResponse);

                return new IndicatorResponse {
                    Candles = candles ?? new List<CandleData>()
                };
            }
            // If the indicator is not for candles
            else {
                // Default behavior for other indicators
                var parsedResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);

                if (parsedResponse == null) {
                    throw new InvalidOperationException("Failed to parse JSON response.");
                }

                return new IndicatorResponse { Data = parsedResponse };
            }

        }
        catch (JsonException ex) {
            throw new InvalidOperationException("Failed to parse JSON response.", ex);
        }

    }//end FromJson()

}// class
