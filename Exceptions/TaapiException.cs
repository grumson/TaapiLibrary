using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Exceptions;
/// <summary>
/// Represents errors that occur during Taapi.io API operations.
/// </summary>
public class TaapiException : Exception {


    /// <summary>
    /// Initializes a new instance of the <see cref="TaapiException"/> class.
    /// </summary>
    public TaapiException() { }


    /// <summary>
    /// Initializes a new instance of the <see cref="TaapiException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TaapiException(string message) : base(message) { }


    /// <summary>
    /// Initializes a new instance of the <see cref="TaapiException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TaapiException(string message, Exception innerException) : base(message, innerException) { }


}// class


/// <summary>
/// Exception for exceeding the maximum allowed rate limit.
/// </summary>
public class RateLimitException : TaapiException {
    public RateLimitException(string message) : base(message) { }
}


/// <summary>
/// Exception for authentication failures.
/// </summary>
public class AuthenticationException : TaapiException {
    public AuthenticationException(string message) : base(message) { }
}

/// <summary>
/// Exception for exceeding the maximum allowed constructs.
/// </summary>
public class ConstructLimitExceededException : TaapiException {
    public ConstructLimitExceededException(string message) : base(message) { }
}

/// <summary>
/// Exception for exceeding the maximum allowed calculations per construct.
/// </summary>
public class CalculationLimitExceededException : TaapiException {
    public CalculationLimitExceededException(string message) : base(message) { }
}
