using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaapiLibrary.Core.Models;

/// <summary>
/// Represents an error response from the Taapi.io API.
/// </summary>
public class ErrorResponse {


    /// <summary>
    /// The error message provided by the API.
    /// </summary>
    public string Error { get; set; } = string.Empty;


    /// <summary>
    /// Parses a raw JSON error response into an ErrorResponse object.
    /// </summary>
    /// <param name="jsonResponse">The raw JSON string representing the error response.</param>
    /// <returns>An ErrorResponse object populated with parsed data.</returns>
    public static ErrorResponse FromJson(string jsonResponse) {

        if (string.IsNullOrWhiteSpace(jsonResponse)) {
            throw new ArgumentException("Response cannot be null or empty", nameof(jsonResponse));
        }

        try {
            return JsonSerializer.Deserialize<ErrorResponse>(jsonResponse)
                   ?? throw new InvalidOperationException("Failed to parse error response.");
        }
        catch (JsonException ex) {
            throw new InvalidOperationException("Invalid JSON format for error response.", ex);
        }

    }//end FromJson()


    /// <summary>
    /// Converts the ErrorResponse object into a readable string representation.
    /// </summary>
    /// <returns>A string summarizing the error details.</returns>
    public override string ToString() {

        return $"Error: {Error}";

    }//end ToString()


}// class
