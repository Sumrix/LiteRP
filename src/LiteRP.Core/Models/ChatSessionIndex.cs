using System;
using System.Collections.Generic;

namespace LiteRP.Core.Models;

public class ChatSessionIndex
{
    public Dictionary<Guid, ChatSessionMetadata> Sessions { get; set; } = new();
}