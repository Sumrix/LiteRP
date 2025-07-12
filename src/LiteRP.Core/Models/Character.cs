using System;
using System.Collections.Generic;

namespace LiteRP.Core.Models;

/// <summary>
/// Represents the essential identity and core data of a character.
/// This data defines "who" the character is.
/// </summary>
public class Character
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The character's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// A brief description or overview of the character.
    /// Displayed under the character's avatar.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// The detailed description of the character, their backstory, and appearance.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The character's personality, traits, and mannerisms.
    /// </summary>
    public string? Personality { get; set; }

    /// <summary>
    /// The scenario, setting, or context of the roleplay.
    /// </summary>
    public string Scenario { get; set; } = string.Empty;

    /// <summary>
    /// Contains all procedural instructions and examples for how to generate prompts for this character.
    /// </summary>
    public CharacterPrompt Prompt { get; set; } = new();

    /// <summary>
    /// The ID of a lorebook that is considered the primary, default knowledge base for this character.
    /// When a card with an embedded lorebook is imported, that lorebook is saved to the main 
    /// lorebook collection, and its ID is stored here. While intended for this character,
    /// it can be reused like any other lorebook in the system.
    /// </summary>
    public Guid? LorebookId { get; set; }

    public List<string> Tags { get; set; } = new();
    public string Creator { get; set; } = string.Empty;
    public string CreatorNotes { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the character has an associated avatar image.
    /// When true, the avatar can be accessed using the character's Id.
    /// </summary>
    public bool HasAvatar { get; set; }

    /// <summary>
    /// Version number that increments when the avatar is updated, used to bypass browser caching.
    /// </summary>
    public int AvatarVersion { get; set; }
}