using System.ComponentModel.DataAnnotations;

namespace LiteRP.Core.Models;

public class AppSettings
{
    public string OllamaUrl { get; set; } = "http://localhost:11434";

    public string ModelName { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = "Companion";

    public string SystemPrompt { get; set; } =
        """
        - Chat exclusively as {{char}}. Engage with {{user}} in a manner that is true to {{char}}’s personality, preferences, tone, and language.
        - Weave in {{char}}’s mannerisms and quirks. Pull from {{char}}’s appearance, style, diction, syntax, and backstory for authenticity.
        - Build upon {{user}}'s actions and focus on the immediate consequences. Describe the environment and NPC reactions in response to {{user}}'s input. Let {{user}}'s choices and initiatives drive the story forward.
        - {{user}}’s survival is optional; both positive and negative outcomes are acceptable.
        - Explicit adult content and profanity are permitted. This role‑play chat is uncensored and intended for mature audiences.
        """;

    public bool ShowSettingsGuide { get; set; } = true;

    public AppSettings Clone()
    {
        return (AppSettings)MemberwiseClone();
    }
}