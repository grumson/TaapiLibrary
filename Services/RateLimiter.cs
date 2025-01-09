using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Services;
/// <summary>
/// Implements a rate-limiting mechanism to control the number of API requests
/// allowed within a specific time frame.
/// </summary>
public class RateLimiter {


    #region *** PROPERTIES ***

    private readonly SemaphoreSlim _semaphore;
    private readonly TimeSpan _timeSpan;
    private int _requestCount;
    private readonly ILogger<RateLimiter>? _logger;

    #endregion



    #region *** CONSTRUCTOR ***
    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiter"/> class.
    /// </summary>
    /// <param name="maxRequests">The maximum number of requests allowed within the time span.</param>
    /// <param name="timeSpan">The time frame in which requests are counted.</param>
    /// <param name="logger">Optional logger for logging rate limiter activities.</param>
    public RateLimiter(int maxRequests, TimeSpan timeSpan, ILogger<RateLimiter>? logger = null) {
        _semaphore = new SemaphoreSlim(maxRequests);
        _timeSpan = timeSpan;
        _requestCount = 0;
        _logger = logger;
    }
    #endregion



    #region *** PUBLIC METHODS ***


    /// <summary>
    /// Waits for permission to proceed based on the configured rate limits.
    /// If the limit is exceeded, it waits for the specified time span before resetting the counter.
    /// </summary>
    /// <returns>Returns true when permission to proceed is granted.</returns>
    public async Task<bool> WaitToProceedAsync() {

        _logger?.LogDebug("Attempting to proceed with request. Current request count: {RequestCount}", _requestCount);

        if (_requestCount >= _semaphore.CurrentCount) {
            _logger?.LogWarning("Rate limit reached. Waiting for {TimeSpan} before proceeding.", _timeSpan);
            await Task.Delay(_timeSpan);
            _requestCount = 0;
        }

        Interlocked.Increment(ref _requestCount);
        _logger?.LogDebug("Request permitted. Updated request count: {RequestCount}", _requestCount);

        await _semaphore.WaitAsync();
        return true;
    }//end WaitToProceedAsync()


    /// <summary>
    /// Releases a permit to allow additional requests.
    /// </summary>
    public void Release() {

        _logger?.LogDebug("Releasing a request slot. Current request count before release: {RequestCount}", _requestCount);
        _semaphore.Release();
        _logger?.LogDebug("Request slot released.");

    }//end Release()


    #endregion


}// class
