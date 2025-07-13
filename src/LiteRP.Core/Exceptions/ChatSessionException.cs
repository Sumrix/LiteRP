using System;

namespace LiteRP.Core.Exceptions;

public class ChatSessionException : Exception
{
    public ChatSessionError ErrorCode { get; }

    public ChatSessionException(ChatSessionError code, string message) : base(message)
    {
        ErrorCode = code;
    }

    public ChatSessionException(ChatSessionError code, string message, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = code;
    }
}