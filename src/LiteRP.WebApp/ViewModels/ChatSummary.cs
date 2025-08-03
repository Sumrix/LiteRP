using LiteRP.Core.Models;

namespace LiteRP.WebApp.ViewModels;

public class ChatSummary
{
    public required Guid Id { get; set; }
    public required Character Character { get; set; }
    public required string LastMessage { get; set; }
    public required TimeSpan Timestamp { get; set; }
}