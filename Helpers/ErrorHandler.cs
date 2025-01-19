using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaapiLibrary.Core.Models;
using TaapiLibrary.Exceptions;

namespace TaapiLibrary.Helpers;
public static class ErrorHandler {

    /// <summary>
    /// Handles API errors by logging and throwing appropriate exceptions.
    /// </summary>
    /// <param name="jsonResponse">The JSON response from the API containing error details.</param>
    /// <param name="logger">Optional logger for logging error details.</param>
    public static void HandleApiError(string jsonResponse, ILogger? logger) {

        // Validate the error response
        if (string.IsNullOrWhiteSpace(jsonResponse)) {
            var errorMessage = "Received an empty or null error response from the API.";
            logger?.LogError(errorMessage);
            throw new TaapiException(errorMessage);
        }

        try {

            // Parse the error response
            var errorResponse = ErrorResponse.FromJson(jsonResponse);
            logger?.LogError("API Error: {ErrorMessage}", errorResponse.Error);

            // Log and categorize specific errors
            switch (errorResponse.Error.ToLowerInvariant()) {
                case "rate limit exceeded":
                    logger?.LogWarning("Rate limit exceeded. Retry after some time.");
                    throw new RateLimitException(errorResponse.Error);

                case "authentication failed":
                    logger?.LogError("Authentication failed. Please verify the API key.");
                    throw new AuthenticationException(errorResponse.Error);

                case "invalid request":
                    logger?.LogError("Invalid request sent to API. Verify parameters.");
                    throw new TaapiException("Invalid request sent to API: " + errorResponse.Error);

                default:
                    logger?.LogError("Unknown API error encountered: {ErrorMessage}", errorResponse.Error);
                    throw new TaapiException("Unknown error: " + errorResponse.Error);
            }
        }
        catch (JsonException ex) {
            logger?.LogError(ex, "Failed to parse API error response JSON.");
            throw new TaapiException("Invalid JSON format in error response.", ex);
        }
        catch (Exception ex) when (ex is not TaapiException) {
            // Log parsing or unexpected exceptions
            logger?.LogError(ex, "Failed to process the API error response.");
            throw new TaapiException("An unexpected error occurred while handling the API error.", ex);
        }

    }//end HandleApiError()


    /// <summary>
    /// Validates limits and throws exceptions for violations.
    /// </summary>
    /// <param name="constructCount">The number of constructs in the request.</param>
    /// <param name="maxConstructs">The maximum allowed constructs.</param>
    /// <param name="maxCalculations">The maximum allowed calculations per construct.</param>
    /// <param name="logger">Optional logger for logging validation errors.</param>
    public static void ValidateLimits(int constructCount, int maxConstructs, List<int> calculationsPerConstruct, int maxCalculations, ILogger? logger) {

        if (constructCount > maxConstructs) {
            var errorMessage = $"Exceeded maximum allowed constructs: {maxConstructs}";
            logger?.LogError(errorMessage);
            throw new ConstructLimitExceededException(errorMessage);
        }

        for (int i = 0; i < calculationsPerConstruct.Count; i++) {
            if (calculationsPerConstruct[i] > maxCalculations) {
                var errorMessage = $"Construct {i + 1} exceeded maximum allowed calculations per construct: {maxCalculations}";
                logger?.LogError(errorMessage);
                throw new CalculationLimitExceededException(errorMessage);
            }
        }

    }//end ValidateLimits()


}// class
