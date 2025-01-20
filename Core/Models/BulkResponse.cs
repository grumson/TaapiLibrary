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
                try {
                    var id = indicatorElement.GetProperty("id").GetString() ?? string.Empty;

                    object result;
                    var resultElement = indicatorElement.GetProperty("result");

                    if (resultElement.ValueKind == JsonValueKind.Array) {
                        // If result is an array, deserialize into a list of dictionaries
                        result = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(resultElement.GetRawText())
                                 ?? new List<Dictionary<string, object>>();
                    }
                    else if (resultElement.ValueKind == JsonValueKind.Object) {
                        // If result is an object, deserialize into a dictionary
                        result = resultElement.EnumerateObject()
                            .ToDictionary(prop => prop.Name, prop => ParseJsonElement(prop.Value));
                    }
                    else {
                        throw new InvalidOperationException("Unexpected result format: Must be an array or object.");
                    }

                    bulkResponse.Data.Add(new BulkIndicatorResponse {
                        Id = id,
                        Result = result
                    });
                }
                catch (Exception ex) {
                    throw new InvalidOperationException($"Error parsing indicator data: {ex.Message}", ex);
                }
            }

            return bulkResponse;
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException) {
            throw new InvalidOperationException("Failed to parse JSON response.", ex);
        }

    }//end FromJson()

    private static object? ParseJsonElement(JsonElement element) {
        return element.ValueKind switch {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Array => element.EnumerateArray().Select(ParseJsonElement).ToList(),
            JsonValueKind.Object => element.EnumerateObject()
                .ToDictionary(prop => prop.Name, prop => ParseJsonElement(prop.Value)),
            _ => null
        };
    }

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
    /// The result data of the indicator (can be a dictionary or a list of dictionary).
    /// </summary>
    public object Result { get; set; } = new();

    /// <summary>
    /// A list of any errors encountered while fetching this indicator.
    /// </summary>
    public List<string> Errors { get; set; } = new();

}
