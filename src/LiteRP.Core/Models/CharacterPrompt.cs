using System.Collections.Generic;

namespace LiteRP.Core.Models;

/// <summary>
/// Contains all data and instructions on "how" to construct a prompt for a character.
/// </summary>
public class CharacterPrompt
{
    /// <summary>
    /// The character's first message to start the conversation.
    /// </summary>
    public List<string> Greetings { get; set; } = new();

    /// <summary>
    /// A sequence of messages demonstrating character speech patterns and behavior.
    /// These are examples, not part of the actual chat history.
    /// </summary>
    public List<string> ExampleOfDialogues { get; set; } = new();

    /// <summary>
    /// A character-specific system prompt.
    /// </summary>
    public string SystemPrompt { get; set; } = string.Empty;

    /// <summary>
    /// Instructions inserted after the conversation history.
    /// </summary>
    public string PostHistoryInstructions { get; set; } = string.Empty;
}