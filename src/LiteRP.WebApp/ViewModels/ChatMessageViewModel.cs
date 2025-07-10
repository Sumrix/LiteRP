using LiteRP.Core.Models;
using Microsoft.Extensions.AI;

namespace LiteRP.WebApp.ViewModels;

public class ChatMessageViewModel
{
    public required MessageSender Sender { get; set; }
    public required string MessageText { get; set; }

    public static ChatMessageViewModel FromChatMessage(ChatMessage message, Character character)
    {
        return new ChatMessageViewModel()
        {
            Sender = message.Role == ChatRole.Assistant
                ? MessageSender.FromAi(character)
                : MessageSender.FromUser("You"),
            MessageText = message.Text
        };
    }
}