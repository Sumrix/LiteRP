using System;

namespace LiteRP.Core.Models;

public class ChatSessionMetadata
{
    public required Guid Id { get; set; }
    public required Guid CharacterId { get; set; }
    public required string LastMessage { get; set; }
    public required DateTime LastModified { get; set; }
}