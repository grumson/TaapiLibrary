using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaapiLibrary.Core.Models;

/// <summary>
/// Represents the response for a bulk request from the Taapi.io API.
/// </summary>
public class BulkResponse {

    /// <summary>
    /// List of individual indicator responses.
    /// </summary>
    public List<BulkIndicatorResponse> Data { get; set; } = new();

    /// <summary>
    /// Parses a raw JSON response into a BulkResponse object.
    /// </summary>
    /// <param name="jsonResponse">The JSON string response from the API.</param>
    /// <returns>A BulkResponse object populated with parsed data.</returns>
    public static BulkResponse FromJson(string jsonResponse) {

        if (string.IsNullOrWhiteSpace(jsonResponse)) {
            throw new ArgumentException("Response cannot be null or empty", nameof(jsonResponse));
        }

        try {
            using var document = JsonDocument.Parse(jsonResponse);
            var root = document.RootElement;

            if (!root.TryGetProperty("data", out var dataElement) || dataElement.ValueKind != JsonValueKind.Array) {
                throw new InvalidOperationException("Invalid JSON structure: Missing or invalid 'data' array.");
            }

            var bulkResponse = new BulkResponse();

            foreach (var indicatorElement in dataElement.EnumerateArray()) {
                var id = indicatorElement.GetProperty("id").GetString() ?? string.Empty;
                var indicatorType = indicatorElement.GetProperty("indicator").GetString() ?? string.Empty;

                object result;

                var resultElement = indicatorElement.GetProperty("result");

                // Handle 'candles' as an array
                if (indicatorType.Equals("candles", StringComparison.OrdinalIgnoreCase) && resultElement.ValueKind == JsonValueKind.Array) {
                    result = resultElement.Deserialize<List<CandleData>>() ?? new List<CandleData>();
                }
                // Handle other indicators as objects
                else if (resultElement.ValueKind == JsonValueKind.Object) {
                    result = resultElement.EnumerateObject()
                        .ToDictionary(prop => prop.Name, prop => (object)prop.Value.ToString());
                }
                else {
                    throw new InvalidOperationException($"Unexpected 'result' format for indicator {indicatorType}.");
                }

                var errors = indicatorElement.TryGetProperty("errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Array
                    ? errorsElement.EnumerateArray().Select(e => e.GetString() ?? string.Empty).ToList()
                    : new List<string>();

                bulkResponse.Data.Add(new BulkIndicatorResponse {
                    Id = id,
                    Result = result,
                    Errors = errors
                });
            }

            return bulkResponse;
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException) {
            throw new InvalidOperationException("Failed to parse JSON response.", ex);
        }

    }//end FromJson()

    // Notranji razred za deserializacijo odziva API-ja
    private class BulkResponseWrapper {
        public List<BulkIndicatorResponseRaw>? Data { get; set; }
    }

    // Surov odziv za posamezni indikator
    private class BulkIndicatorResponseRaw {
        public string Id { get; set; } = string.Empty;
        public Dictionary<string, object> Result { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }

}

/// <summary>
/// Represents the response for a single indicator within a bulk request.
/// </summary>
public class BulkIndicatorResponse {

    /// <summary>
    /// The unique identifier of the indicator result.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The result data of the indicator (can be a dictionary or a list of candles).
    /// </summary>
    public object Result { get; set; } = new();

    /// <summary>
    /// A list of any errors encountered while fetching this indicator.
    /// </summary>
    public List<string> Errors { get; set; } = new();

}
