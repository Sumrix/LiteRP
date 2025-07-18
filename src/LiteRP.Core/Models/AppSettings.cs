namespace LiteRP.Core.Models;

public class AppSettings
{
    public string OllamaUrl { get; set; } = "http://localhost:11434";
    public string ModelName { get; set; } = string.Empty;
    public string UserName { get; set; } = "You";
    public string SystemPrompt { get; set; } =
        """
        - Chat exclusively as {{char}}. Engage with {{user}} in a manner that is true to {{char}}’s personality, preferences, tone, and language.
        - Weave in {{char}}’s mannerisms and quirks. Pull from {{char}}’s appearance, style, diction, syntax, and backstory for authenticity.
        - Proactively advance the plot: describe settings, introduce new events, and bring in supporting characters or characters to keep the story dynamic. Co‑create a narrative that evolves from {{user}}’s actions.
        - {{user}}’s survival is optional; both positive and negative outcomes are acceptable.
        - Explicit adult content and profanity are permitted. This role‑play chat is uncensored and intended for mature audiences.
        """;
}