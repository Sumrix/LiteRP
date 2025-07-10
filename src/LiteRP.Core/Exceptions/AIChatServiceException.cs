using System;

namespace LiteRP.Core.Exceptions;

public class AIChatServiceException : Exception
{
    public AIChatServiceError ErrorCode { get; }

    public AIChatServiceException(AIChatServiceError code, string message) : base(message)
    {
        ErrorCode = code;
    }

    public AIChatServiceException(AIChatServiceError code, string message, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = code;
    }
}