using System.Collections.Generic;
using System.Text.Json.Serialization;
using LiteRP.Core.Models;
using LiteRP.Core.Models.CharacterCard;

namespace LiteRP.Core.Helpers;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Character))]
[JsonSerializable(typeof(TavernCardV1))]
[JsonSerializable(typeof(TavernCardV2))]
[JsonSerializable(typeof(List<ChatMessage>))]
[JsonSerializable(typeof(ChatSessionIndex))]
[JsonSerializable(typeof(Lorebook))]
[JsonSerializable(typeof(AppSettings))]
internal partial class AppJsonContext : JsonSerializerContext { }