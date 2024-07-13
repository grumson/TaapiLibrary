using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Exceptions;

// Exception that is thrown when the rate limit is exceeded
public class RateLimitExceededException : Exception {

    public double RetryAfterSeconds { get; }

    public RateLimitExceededException(string message, double retryAfterSeconds)
        : base(message) {
        RetryAfterSeconds = retryAfterSeconds;
    }

}// class RateLimitExceededException
