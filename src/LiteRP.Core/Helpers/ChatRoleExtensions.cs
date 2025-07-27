using System.Collections.Generic;
using System.ComponentModel;
using LiteRP.Core.Enums;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LiteRP.Core.Helpers;

public static class ChatRoleExtensions
{
    private static readonly Dictionary<AuthorRole, ChatRole> AuthorRoleToChatRole = new()
    {
        [AuthorRole.System] = ChatRole.System,
        [AuthorRole.Assistant] = ChatRole.Assistant,
        [AuthorRole.User] = ChatRole.User,
    };

    public static AuthorRole ToAuthorRole(this ChatRole chatRole) =>
        chatRole switch
        {
            ChatRole.System => AuthorRole.System,
            ChatRole.Assistant => AuthorRole.Assistant,
            ChatRole.User => AuthorRole.User,
            _ => throw new InvalidEnumArgumentException($"Unexpected chat role '{chatRole}'")
        };

    public static ChatRole ToChatRole(this AuthorRole authorRole) => AuthorRoleToChatRole[authorRole];
}