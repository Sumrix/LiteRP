using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteRP.Core.Models;

public class ChatSessionState
{
    public required Guid Id { get; set; }
    public required Guid CharacterId { get; set; }
    public required List<ChatMessage> Messages { get; set; }
    public required DateTime LastModified { get; set; }

    public ChatSessionMetadata GetMetadata()
    {
        return new ChatSessionMetadata()
        {
            Id = Id,
            CharacterId = CharacterId,
            LastMessage = Messages.Last().Text,
            LastModified = LastModified
        };
    }
}