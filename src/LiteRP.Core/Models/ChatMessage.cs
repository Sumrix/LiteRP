using LiteRP.Core.Enums;

namespace LiteRP.Core.Models;

public record ChatMessage
(
    ChatRole Role,
    string Text
);