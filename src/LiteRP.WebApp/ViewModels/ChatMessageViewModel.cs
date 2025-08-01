using System.ComponentModel;
using LiteRP.Core.Enums;
using LiteRP.Core.Models;

namespace LiteRP.WebApp.ViewModels;

public class ChatMessageViewModel
{
    public required MessageSender Sender { get; set; }
    public MessageDisplayMode Mode { get; set; }
    public required string MessageText { get; set; }

    public static ChatMessageViewModel FromChatMessage(ChatMessage message, string userName, Character character) =>
        message.Role switch
        {
            ChatRole.Assistant => AiMessage(message.Text, character),
            ChatRole.User => UserMessage(userName, message.Text),
            _ => throw new InvalidEnumArgumentException($"Unexpected message role {message.Role}.")
        };

    public static ChatMessageViewModel UserMessage(string userName, string message) =>
        new()
        {
            Sender = MessageSender.FromUser(userName),
            MessageText = message,
            Mode = MessageDisplayMode.Ready,
        };

    public static ChatMessageViewModel AiMessage(string message, Character character) =>
        new()
        {
            Sender = MessageSender.FromAi(character),
            MessageText = message,
            Mode = MessageDisplayMode.Ready,
        };
}