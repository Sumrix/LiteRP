using System;
using System.Collections.Generic;
using Microsoft.Extensions.AI;

namespace LiteRP.Core.Models;

public class ChatSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public List<ChatMessage> Messages { get; set; } = new();
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}