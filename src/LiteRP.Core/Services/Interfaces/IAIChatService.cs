using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.AI;

namespace LiteRP.Core.Services.Interfaces;

public interface IAIChatService
{
    IAsyncEnumerable<string> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        CancellationToken cancellationToken = default);
}