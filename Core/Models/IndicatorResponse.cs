using System.Text.Json;

namespace TaapiLibrary.Core.Models;
/// <summary>
/// Represents the response from the Taapi.io API for an indicator.
/// </summary>
public class IndicatorResponse {

    /// <summary>
    /// Primary data of the indicator response.
    /// This is a dictionary where keys are parameter names and values are their respective values.
    /// </summary>
    public Dictionary<string, object> Data { get; private set; } = new();


    /// <summary>
    /// Parses a raw JSON response from the API into an IndicatorResponse object.
    /// </summary>
    /// <param name="jsonResponse">The JSON response from the API as a string.</param>
    /// <returns>An IndicatorResponse object populated with the API response data.</returns>
    public static IndicatorResponse FromJson(string jsonResponse) {

        if (string.IsNullOrWhiteSpace(jsonResponse)) {
            throw new ArgumentException("Response cannot be null or empty", nameof(jsonResponse));
        }

        // Deserialize the JSON response into a dictionary
        var parsedResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);

        if (parsedResponse == null) {
            throw new InvalidOperationException("Failed to parse JSON response.");
        }

        return new IndicatorResponse { Data = parsedResponse };
    }//end FromJson()


    /// <summary>
    /// Retrieves a specific parameter value from the response data.
    /// </summary>
    /// <param name="key">The parameter key to retrieve.</param>
    /// <typeparam name="T">The expected type of the value.</typeparam>
    /// <returns>The value of the parameter if found; otherwise, default value of T.</returns>
    public T GetValue<T>(string key) {

        if (Data.TryGetValue(key, out var value) && value is T typedValue) {
            return typedValue;
        }

        return default!;
    }//end GetValue()


}// class
