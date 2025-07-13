namespace LiteRP.Core.Models;

public record ChatMessage
(
    ChatRole Role,
    string Text
);