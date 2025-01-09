

# Taapi Library

## Overview

The Taapi class library is a .NET library designed to provide a simple, structured interface for interacting with the [Taapi.io API](https://taapi.io). It supports all of technical indicators, making it an ideal choice for financial applications requiring market analysis. This library also includes advanced features like bulk requests, robust error handling, and Api rate, calculation and construct limit management.

**Table of Contents**
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Extensibility](#extensibility)
- [Contributing](#contributing)
- [License](#license)
- [Support](#support)
- [Changelog](#changelog)

---

## Features

- **Comprehensive Indicator Support:** Fetch technical indicators (e.g., RSI, MACD, EMA).
- **Single Indicator Requests:** Query a single indicator for a given symbol and interval.
- **Bulk Requests:** Query multiple indicators simultaneously.
- **Bulk Multi-Construct Requests:** Query multiple indicators for multiple constructs simultaneously.
- **Rate Limiting:** Built-in API rate limiter to comply with Taapi.io usage limits asoociated with your subscription tier.
- **Calculation and Constructs Limit Management:** Handle calculation and construct limits to comply with Taapi.io usage limits associated with your subscription tier.
- **Robust Centralized Error Handling:** Robust error management using the ErrorHandler class, providing specific exceptions for common issues like rate limits and authentication errors.
- **Extensibility:** Easily extend to support additional endpoints or exchanges.

---

## Installation

Add the library to your .NET project using one of the following methods:

```bash

// .NET CLI
> dotnet add package Grumson.Utilities.Taapi --version 2.0.0

// Package Manager
PM> NuGet\Install-Package Grumson.Utilities.Taapi -Version 2.0.0

// Package Reference
<PackageReference Include="Grumson.Utilities.Taapi" Version="2.0.0" />

```

Or, include the project in your solution and reference it directly.

---

## Usage

### Initialize the Client

```csharp
using Grumson.Taapi.Services;
using Grumson.Taapi.Core.Enums;
using Microsoft.Extensions.Logging;

ILogger<TaapiClient> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<TaapiClient>();
string apiKey = "YOUR_API_KEY";
SubscriptionType subscriptionType = SubscriptionType.Pro;
var client = new TaapiClient(logger, apiKey, subscriptionType);
```

### Fetch a Single Indicator

```csharp
try {

    var request = new IndicatorRequest
    {
        Exchange = "binance",
        Symbol = "BTC/USDT",
        Interval = "1h",
        Indicator = "rsi"
    };

    IndicatorResponse response = await client.GetIndicatorAsync(request);
    Console.WriteLine($"RSI Value: {response.Data["value"]}");

} 
catch (RateLimitException ex)
{
    Console.WriteLine("Rate limit exceeded: " + ex.Message);
}
catch (AuthenticationException ex)
{
    Console.WriteLine("Authentication failed: " + ex.Message);
}
catch (TaapiException ex)
{
    Console.WriteLine("API error: " + ex.Message);
}
```

### Bulk Indicator Request

```csharp
try
{

    // Build bulk request for multiple indicators
    var bulkRequest = new BulkRequest {
        Constructs = new List<Construct>
        {
            new Construct
            {
                Exchange = "binance",
                Symbol = "BTC/USDT",
                Interval = TimeFrame.FourHours.GetDescription(),
                Indicators = new List<IndicatorDetail>
                {
                    new IndicatorDetail { Id = "BTC_rsi", Indicator = "rsi" },
                    new IndicatorDetail { Id = "BTC_macd", Indicator = "macd" }
                }
            }
        }
    };

    BulkResponse bulkResponse = await client.GetBulkAsync(bulkRequest);
    foreach (var data in bulkResponse.Data){

        // Split the indicator ID to get the symbol and indicator name
        string[] idParts = data.Id.Split('_');
        string symbol = idParts[0];
        string indicator = idParts[1];

        // Print the symbol and indicator name
        Console.WriteLine($"Symbol: {symbol}, Indicator: {indicator}");

        // Print the indicator results
        foreach (var result in data.Result) {
            Console.WriteLine($"{result.Key}: {result.Value}");
        }

        Console.WriteLine();
    }

}
catch (CalculationLimitExceededException ex)
{
    Console.WriteLine("Calculation limit error: " + ex.Message);
}
catch (RateLimitException ex)
{
    Console.WriteLine("Rate limit exceeded: " + ex.Message);
}
catch (AuthenticationException ex)
{
    Console.WriteLine("Authentication failed: " + ex.Message);
}
catch (TaapiException ex)
{
    Console.WriteLine("API error: " + ex.Message);
}
```

### Bulk indicators for multiple constructs

```csharp
try
{

        var bulkRequest = new BulkRequest {
        Constructs = new List<Construct>
        {
            new Construct
            {
                Exchange = "binance",
                Symbol = "BTC/USDT",
                Interval = "1h",
                Indicators = new List<IndicatorDetail>
                {
                    new IndicatorDetail { Id = "BTC_rsi_1h", Indicator = "rsi" },
                    new IndicatorDetail { Id = "BTC_macd_1h", Indicator = "macd" }
                }
            },
            new Construct
            {
                Exchange = ExchangeType.Binance.GetDescription(),
                Symbol = "ETH/USDT",
                Interval = TimeFrame.FourHours.GetDescription(),
                Indicators = new List<IndicatorDetail>
                {
                    new IndicatorDetail { Id = "ETH_rsi_4h", Indicator = "rsi" },
                    new IndicatorDetail { Id = "ETH_macd_4h", Indicator = "macd" }
                }
            }
        }
    };


    var response = await client.GetBulkAsync(bulkRequest);

    foreach (var data in response.Data) {

        // Split the indicator ID to get the symbol, indicator name and time frame
        string[] idParts = data.Id.Split('_');
        string symbol = idParts[0];
        string indicator = idParts[1];
        string interval = idParts[2];

        // Print the symbol and indicator name
        Console.WriteLine($"Symbol: {symbol}, Indicator: {indicator}, Interval: {interval}");

        // Print the indicator results
        foreach (var result in data.Result) {
            Console.WriteLine($"{result.Key}: {result.Value}");
        }

        Console.WriteLine();
    }

}
catch (ConstructLimitExceededException ex)
{
    Console.WriteLine("Construct limit error: " + ex.Message);
}
catch (CalculationLimitExceededException ex)
{
    Console.WriteLine("Calculation limit error: " + ex.Message);
}
catch (RateLimitException ex)
{
    Console.WriteLine("Rate limit exceeded: " + ex.Message);
}
catch (AuthenticationException ex)
{
    Console.WriteLine("Authentication failed: " + ex.Message);
}
catch (TaapiException ex)
{
    Console.WriteLine("API error: " + ex.Message);
}
```

### Test API Connection

```csharp
bool isConnected = await client.TestConnectionAsync();
Console.WriteLine(isConnected ? "Connection Successful" : "Connection Failed");
```

### Error Handling
```csharp
try
{
    var response = await client.GetIndicatorAsync(request);
}
catch (ConstructLimitExceededException ex)
{
    Console.WriteLine("Construct limit error: " + ex.Message);
}
catch (CalculationLimitExceededException ex)
{
    Console.WriteLine("Calculation limit error: " + ex.Message);
}
catch (RateLimitException ex)
{
    Console.WriteLine("Rate limit exceeded: " + ex.Message);
}
catch (AuthenticationException ex)
{
    Console.WriteLine("Authentication failed: " + ex.Message);
}
catch (TaapiException ex)
{
    Console.WriteLine("API error: " + ex.Message);
}
```

### Enum Usage for Configuration
Enums are used extensively to represent various types in the library and standardize and simplify request configurations.
```csharp
TimeFrame interval = TimeFrame.OneHour;
string intervalName = interval.GetDescription(); // "1h"

// Configure the bulk request with enum values
var bulkRequest = new BulkRequest {
    Constructs = new List<Construct>
    {
        new Construct
        {
            Exchange = ExchangeType.Binance.GetDescription(),
            Symbol = "BTC/USDT",
            Interval = TimeFrame.FourHours.GetDescription(),
            Indicators = new List<IndicatorDetail>
            {
                new IndicatorDetail { Id = "BTC_rsi_4h", Indicator = IndicatorType.RSI.GetDescription() },
                new IndicatorDetail { Id = "BTC_macd_4h", Indicator = IndicatorType.MACD.GetDescription() }
            }
        }
    }
};
```

---

## Project Structure

- **Core**: Contains models, enums, and default configurations.
  - **Models**: `IndicatorRequest`, `IndicatorResponse`, `BulkRequest`, `BulkResponse`
  - **Enums**: `IndicatorType`, `TimeFrame`, `ExchangeType`, etc.
- **Services**: Includes the main API client (`TaapiClient`) and supporting classes like `RateLimiter`.
- **Helpers**: Utility classes for parsing and formatting data.
  - **ErrorHandler**: A utility class for parsing and managing API errors, throwing specific exceptions for better debugging and handling.
- **Exceptions**: Custom exception classes for error handling.

---

## Extensibility

The library is designed to be extensible. You can:

1. Add support for new endpoints by extending `ITaapiClient` and `TaapiClient`.
2. Include additional indicators or exchanges by updating the respective enums.
3. Modify rate limits or subscription tiers in `ApiRateLimits.cs`.

---

## Contributing

Contributions are welcome! 
If you have any questions, suggestions, or you want to contribute to this project, feel free to contact me at <info@grumson.eu>

---

## License

This project is licensed under the MIT License. See `LICENSE` for more information.

---

## Support

If you like this project and you want to support it, you can donate to the following addresses:

**Network BSC BNB smart chain (BEP20)** : 0xd8c509ed7d8f96847618d926e2b831d804e02ece
- BNB : 0xd8c509ed7d8f96847618d926e2b831d804e02ece
- USDT : 0xd8c509ed7d8f96847618d926e2b831d804e02ece

**Network Solana (SPL)** : 4D1W3Vv2tbfAzuEgBSiNGqdtGT5wUjbodoF6mXEsnvTf
- SOL : 4D1W3Vv2tbfAzuEgBSiNGqdtGT5wUjbodoF6mXEsnvTf
- USDC : 4D1W3Vv2tbfAzuEgBSiNGqdtGT5wUjbodoF6mXEsnvTf

**Network Ethereum (ERC20)** : 0xd8c509ed7d8f96847618d926e2b831d804e02ece
- ETH : 0xd8c509ed7d8f96847618d926e2b831d804e02ece
- USDC : 0xd8c509ed7d8f96847618d926e2b831d804e02ece

**BTC** : 19pxXzh1Kzzw73v6iKbowr1DJro5ozgZj6

---

## Changelog


#### Version 2.0.0 
It is totally rewritten from scratch. The library is now more flexible and extensible. It supports multiple indicators and multiple constructs in bulk requests. It has a new error handling system and a new API rate limiter system.
