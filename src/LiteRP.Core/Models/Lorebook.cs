using System;
using System.Collections.Generic;

namespace LiteRP.Core.Models;

/// <summary>
/// Represents a Lorebook. Can be a standalone entity or part of a Character.
/// </summary>
public class Lorebook
{
    /// <summary>
    /// Unique identifier for the lorebook. Essential for standalone books.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The display name of the lorebook.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// A description of the lorebook's purpose or content.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The list of entries contained in this lorebook.
    /// </summary>
    public List<LorebookEntry> Entries { get; set; } = new();

    /// <summary>
    /// Determines if this lorebook is active and should be used during prompt generation.
    /// Especially useful for globally available, standalone lorebooks.
    /// </summary>
    //public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// How many of the most recent chat messages to scan for keywords.
    /// </summary>
    public int? ScanDepth { get; set; }

    /// <summary>
    /// The maximum number of tokens this lorebook is allowed to inject into the context.
    /// </summary>
    public int? TokenBudget { get; set; }

    /// <summary>
    /// If true, the content of an inserted entry can itself trigger other entries.
    /// </summary>
    public bool? RecursiveScanning { get; set; }
}