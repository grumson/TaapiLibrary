using System.Text.Json;

namespace TaapiLibrary.Core.Models;
/// <summary>
/// Represents the response from the Taapi.io API for an indicator.
/// </summary>
public class IndicatorResponse {

    /// <summary>
    /// Dictionary of the indicator response data. For single indicator requests.
    /// </summary>
    public Dictionary<string, object> Data { get; private set; } = new();


    /// <summary>
    /// List of dictionaries containing the indicator response data. For list of indicators requests.
    /// </summary>
    public List<Dictionary<string, object>>? DataList { get; private set; }


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

            var indicatorResponse = new IndicatorResponse();
            using var document = JsonDocument.Parse(jsonResponse);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Array) {
                // Deserialize the JSON array into a list of dictionaries
                indicatorResponse.DataList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonResponse)
                                         ?? new List<Dictionary<string, object>>();
            }
            else if (root.ValueKind == JsonValueKind.Object) {
                // Deserialize the JSON object into a dictionary
                var parsedResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);

                if (parsedResponse == null) {
                    throw new InvalidOperationException("Failed to parse JSON response.");
                }

                indicatorResponse.Data = parsedResponse;
            }
            else {
                throw new InvalidOperationException("Unexpected JSON structure: Root element must be an array or object.");
            }

            return indicatorResponse;
        }
        catch (JsonException ex) {
            throw new InvalidOperationException("Failed to parse JSON response.", ex);
        }

    }//end FromJson()

}// class
