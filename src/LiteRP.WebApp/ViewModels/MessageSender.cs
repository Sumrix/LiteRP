using LiteRP.Core.Models;

namespace LiteRP.WebApp.ViewModels;

public class MessageSender
{
    public SenderType SenderType { get; }
    public string UserName { get; }
    public Character? AiCharacter { get; }
    public string SenderName => SenderType == SenderType.Ai ? AiCharacter!.Name : UserName;

    private MessageSender(SenderType senderType, string userName, Character? character)
    {
        SenderType = senderType;
        UserName = userName;
        AiCharacter = character;
    }

    public static MessageSender FromUser(string userName)
    {
        return new MessageSender(SenderType.User, userName, null);
    }

    public static MessageSender FromAi(Character character)
    {
        return new MessageSender(SenderType.Ai, string.Empty, character);
    }
}