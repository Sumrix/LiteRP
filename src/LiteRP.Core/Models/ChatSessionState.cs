using System;
using System.Collections.Generic;

namespace LiteRP.Core.Models;

public class ChatSessionState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public List<ChatMessage> Messages { get; set; } = new();
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}